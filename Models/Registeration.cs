namespace MvcApp.Models;
using System.ComponentModel.DataAnnotations;

public class Registeration
{
    [Required(ErrorMessage = "Enter your Name")]
    public string? Name {get; set;}

    [Required(ErrorMessage = "Enter your Mail")]
    [EmailAddress]
    public string? Email {get; set;}

    [Required(ErrorMessage = "Password is required")]
    [MinLength(2)]
    public string? Password {get; set;}
}