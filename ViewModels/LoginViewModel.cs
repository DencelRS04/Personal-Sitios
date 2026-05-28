using System.ComponentModel.DataAnnotations;

namespace Personal_Sitios.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar el usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public string Password { get; set; }
    }
}