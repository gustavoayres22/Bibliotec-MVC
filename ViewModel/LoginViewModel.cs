using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace BibliotecMVC.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "informe o e-mail.")]
    [EmailAddress(ErrorMessage = "informe um e-mail válido.")]

    public string Email {get; set;} = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha {get; set;} = string.Empty;
}