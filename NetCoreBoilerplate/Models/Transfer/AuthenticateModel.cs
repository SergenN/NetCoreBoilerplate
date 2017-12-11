using System.ComponentModel.DataAnnotations;

namespace NetCoreBoilerplate.Models.Transfer
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AuthenticateModel : RegisterModel
    {  
        public bool Remember { get; set; }
    }

    public class ResetPasswordModel : RegisterModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Code { get; set; }
    }

    public class ConfirmMailModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string UserId { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}