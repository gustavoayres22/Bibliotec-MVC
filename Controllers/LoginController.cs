using Microsoft.AspNetCore.Mvc;
namespace BibliotecMVC.Controllers
{
public class LoginController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

//recebe s dados enviados pelo formulário
    [HttpPost]

    public IActionResult Index(LoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
        }
}
}