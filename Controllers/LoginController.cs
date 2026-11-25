namespace BibliotecMVC.Controllers

public class BibliotecMVC.Controllers;
public class LoginController : Controllers
{
    [HttpGet]
    public IActionResult Index()
    {
        return view();
    }
}