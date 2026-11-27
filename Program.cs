using BibliotecMVC.Data;
using BibliotecMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexaoPadrao")
    )
);

// Configura a autenticação utilizando coockies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {

        //Página para qual o usuario que será direcionado caso não esteja autenticado
        options.LoginPath = "/Login/Index";

        //Página para qualo usuário será direcionado caso nça esteja permissão
        options.AccessDeniedPath = "/Login/AcessoNegado";

        //Define o tempo de validde de auntenticação
        //Inatividade
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        //Remova a validade da autenticação conforme a atividade do usuário
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

var app = builder.Build();

// //! CODIGO TEMPORARIO PARA MUDAR AS SENHAS E FICAR IDENTIFICAVEL
// // Código temporário para atualizar as senhas de teste.
// using (var scope = app.Services.CreateScope())
// {
//     // Obtém o contexto do banco de dados.
//     var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//     // Obtém o serviço responsável pela geração do hash.
//     var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

//     // Busca os usuários que ainda possuem a senha de teste em texto puro.
//     var usuarios = contexto.Usuario
//         .Where(u => u.Senha == "admin456")
//         .ToList();

//     foreach (var usuario in usuarios)
//     {
//         // Gera o hash da senha do usuário.
//         usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);
//     }

//     // Salva as alterações no banco de dados.
//     contexto.SaveChanges();
// }


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

//A ordem é importate => primeiro identifica o usuário, depois ve se ele tem permissão

//Identifica o usuáio do cookie de autenticação
app.UseAuthentication();

//Verifica se o usuário possuipara acesar o recurso solicitado
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
