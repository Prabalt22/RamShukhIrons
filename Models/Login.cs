using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Login
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string? Email {get; set;}

    [Required(ErrorMessage = "Password is required")]
    [MinLength(2)]
    public string? Password {get; set;}
}