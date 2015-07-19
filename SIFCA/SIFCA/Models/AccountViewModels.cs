using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIFCA.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage="Este campo es requerido")]
        [Display(Name = "Usuario")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar credenciales")]
        public bool RememberMe { get; set; }
    }

}
