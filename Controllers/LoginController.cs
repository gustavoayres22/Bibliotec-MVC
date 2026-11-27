using System.Security.Claims;
using BibliotecMVC.Data;
using BibliotecMVC.Models;
using BibliotecMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace BibliotecMVC.Controllers
{
    public class LoginController : Controller
    {
        //RECEBENDO DEPÊNDENCIAS (registradas no programa)
        //Permite consultar e manipular os dados do banco
        private readonly AppDbContext context;

        private readonly IPasswordHasher<Usuario> passwordHasher;

        public LoginController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //recebe os dados enviados pelo formulário
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Usuario usuario = await context.Usuario.FirstOrDefaultAsync(u => u.Email == model.Email && u.Excluido == false);

            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail ou senha inválidos");
                return View(model);
            }

            var resultadoSenha = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Senha,
                model.Senha
            );

            if (resultadoSenha == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "E-mail ou senha inválidos.");
                return View(model);
            }

            if (resultadoSenha == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.Senha = passwordHasher.HashPassword(usuario, model.Senha);

                await context.SaveChangesAsync();
            }

            bool administrador = await context.Administrador.AnyAsync(a => a.IdAdministrador == usuario.IdUsuario);

            //verifica se o usuario está cadastrado como aluno
            bool aluno = await context.Aluno.AnyAsync(a => a.IdAluno == usuario.IdUsuario);

            //Coloca o tipo do perfil de usuario na variavel tipoUsuario
            string? tipoUsuario = administrador ? "Administrador" : aluno ? "Aluno" : null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),

                new Claim(ClaimTypes.Name, usuario.Nome),

                new Claim(ClaimTypes.Email, usuario.Email),

                new Claim(ClaimTypes.Role, tipoUsuario)
            };

            var identidade = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            //Representa o usuario autenticado
            //Classe do asp.net para representar o usuario
            var principal = new ClaimsPrincipal(identidade);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal
            );

            return RedirectToAction("Index","Home");
        }
    }
}