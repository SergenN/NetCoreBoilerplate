using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreBoilerplate.Models.Database;
using NetCoreBoilerplate.Models.Transfer;
using NetCoreBoilerplate.Services.Auth;
using NetCoreBoilerplate.Services.Messaging;

namespace NetCoreBoilerplate.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ILoggerFactory loggerFactory, IAuthService authService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AuthController>();
            _authService = authService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            
            if (!result.Succeeded)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _userManager.FindByEmailAsync(model.Email);
            var claims = _authService.GenerateClaimsIdentity(appUser.Email, appUser.Id);
            return Ok(_authService.GenerateEncodedToken(appUser.Email, claims, model.Remember));
        }
       
        [HttpPost("register")]
        public async Task<object> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = new ApplicationUser()
            {
                UserName = model.Email, 
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(ModelState);
            }
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by clicking this link: {callbackUrl}");
            _logger.LogInformation(3, "User created a new account with password.");
            return Ok(result);
        }
        
        [HttpGet("Confirm")]
        public async Task<IActionResult> ConfirmEmail(ConfirmMailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok();
        }
        
        [HttpPost("RequestReset")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            return Ok();
        }

        [HttpPost("ConfirmReset")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid reset attempt.");
                return BadRequest(ModelState);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result);
        }
    }
}