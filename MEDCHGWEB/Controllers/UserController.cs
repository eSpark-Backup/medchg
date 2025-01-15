using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Migrations;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Repository.User;
using MEDCHGClassLibrary.Utilities.Enum;
using MEDCHGWEB.Components.Account;
using MEDCHGWEB.Components.Account.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using static MEDCHGWEB.Components.Pages.Admin.RegisterAdminUser;
using static MEDCHGWEB.Components.Pages.User.EditUser;


namespace MEDCHGWEB.Controllers
{
    [Authorize(Policy = "ApiPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUsersHospital _userHospitalRepository;
        private readonly IUsersDiagnosticCode _userDiagnosticCodeRepository;
        private readonly IUsersChargeCode _userChargeCodeRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IdentityRedirectManager _redirectManager;
        private IEnumerable<IdentityError>? identityErrors;
        public UserController(IUserRepository userRepository, IUsersHospital userHospitalRepository, IUsersDiagnosticCode userDiagnosticCodeRepository, IUsersChargeCode userChargeCodeRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<UserController> logger, IdentityRedirectManager redirectManager, IUserStore<ApplicationUser> userStore)
        {
            _userRepository = userRepository;
            _userHospitalRepository = userHospitalRepository;
            _userDiagnosticCodeRepository = userDiagnosticCodeRepository;
            _userChargeCodeRepository = userChargeCodeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _redirectManager = redirectManager;
            _userStore = userStore;
        }
        // GET: api/<AdminUser>
        [HttpGet("Get")]
        public async Task<ActionResult<User>> GetAll()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("GetAllUsersByProviderIdAsync/{Id}")]
        public async Task<ActionResult<User>> GetAllUsersByProviderIdAsync(int Id)
        {
            var users = await _userRepository.GetAllUsersByProviderIdAsync(Id);
            return Ok(users);
        }
        [HttpGet("GetUserByProviderAndUserName")]
        public async Task<ActionResult<bool>> GetUserByProviderAndUserName(int providerId, string userName)
        {
            var user = await _userRepository.GetUserByProviderAndUsername(providerId, userName);
            if (user != null)
                return Ok(true);
            else return Ok(false);
        }
        [HttpPost("DeleteUser")]
        public async Task<ActionResult> DeleteUserAsync([FromBody] int id)
        {
            var asp = await _userRepository.GetUserById(id);
            var aspuser = await _userManager.FindByIdAsync(asp.UserId);
            if (aspuser != null)
            {
                await _userManager.DeleteAsync(aspuser);
                var user = await _userRepository.DeleteUserAsync(id);
                return Ok(user);
            }
            return BadRequest();
        }
        [HttpPut("StatusChange")]
        public async Task<ActionResult> StatusChange([FromBody] User model)
        {
            var provider = await _userRepository.UpdateUserAsync(model);
            return Ok(provider);
        }
        [HttpPut("StatusChangeForAll")]
        public async Task<ActionResult> StatusChangeForAll([FromBody] List<User> model)
        {
            var provider = await _userRepository.UpdateAllUserAsync(model);
            if(provider.Success == true) return Ok(provider);
            return BadRequest(provider);
        }
        [HttpPost("Register")]
        [AllowAnonymous]

        public async Task<IActionResult> AddUser(UserDTO model)
        {
            var aspuser = CreateUser();
            var emailStore = GetEmailStore();
            aspuser.EmailConfirmed = true;
            await emailStore.SetEmailAsync(aspuser, model.Email, CancellationToken.None);
            aspuser.ProviderName = model.ProviderName;
            IdentityResult role;
            if (model.UserRole is not "Editor Access" && model.UserRole is not "Operator Access")
            {
                await _userStore.SetUserNameAsync(aspuser, model.UserName.ToLower() + "." + model.ProviderName, CancellationToken.None);
            }
            else
            {
                await _userStore.SetUserNameAsync(aspuser, model.UserName, CancellationToken.None);
            }
            var result = await _userManager.CreateAsync(aspuser, model.Password);
            role = await _userManager.AddToRoleAsync(aspuser, model.UserRole);
            if (!result.Succeeded && !role.Succeeded)
            {
                identityErrors = result.Errors;
                return BadRequest();
            }
            _logger.LogInformation("User created a new account with password.");
            model.UserId = aspuser.Id;
            model.Email = aspuser.Email;
            var user = await _userRepository.AddUserAsync(model);
            return Ok(user);
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
        // GET api/<UserController>/5
        [HttpPost("GetById{id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(int id)
        {
            var users = await _userRepository.GetUserById(id);
            return Ok(users);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser([FromBody] LoginDTO model)
        {
            var user = await _userRepository.GetUserByLogin(model.UserName, model.ProviderName);
            return Ok(user);
        }

        [HttpPost("LoginAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdmin([FromBody] string userName)
        {
            var user = await _userRepository.GetAdminByLogin(userName);
            return Ok(user);
        }
        [HttpGet("GetUsersChargeCodeById{id}")]
        public async Task<ActionResult<UsersChargeCode>> GetUsersChargeCodeByIdAsync(int id)
        {
            var users = await _userChargeCodeRepository.GetUsersChargeCodeByIdAsync(id);
            return Ok(users);
        }
        [HttpGet("GetUsersDiagnosticCodeById{id}")]
        public async Task<ActionResult<UsersDiagnosticCode>> GetUsersDiagnosticCodeByIdAsync(int id)
        {
            var users = await _userDiagnosticCodeRepository.GetUsersDiagnosticCodeByIdAsync(id);
            return Ok(users);
        }
        [HttpGet("GetUsersHospitalById{id}")]
        public async Task<ActionResult<UsersHospital>> GetUsersHospitalByIdAsync(int id)
        {
            var users = await _userHospitalRepository.GetUsersHospitalByIdAsync(id);
            return Ok(users);
        }
        [HttpPost("UpdateUserPassword{id}")]
        public async Task<string> UpdateUserPassword(string id, [FromBody] UserDTOs model)
        {
            string ResetPassword;
            var AspUser = await _userManager.FindByIdAsync(id);
            IdentityResult? changePasswordResult;
            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                var hasPassword = await _userManager.HasPasswordAsync(AspUser);
                if (!hasPassword)
                {
                    ResetPassword = "Enter password is incorrect.";
                }
                changePasswordResult = await _userManager.ChangePasswordAsync(AspUser, model.OldPassword, model.Password);
            }
            else
            {
                changePasswordResult = await _userManager.ResetPasswordAsync(AspUser, model.Code, model.Password);
            }
            if (!changePasswordResult.Succeeded)
            {
                ResetPassword = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
                return ResetPassword;
            }
            await _signInManager.RefreshSignInAsync(AspUser);
            return "All Okay!";
        }
        [HttpPut("UpdateUser{id}")]
        public async Task<ActionResult<User>> Put(int id, [FromBody] UserDTO model)
        {
            var entity = new User
            {
                Id = id,
                ImgUrl = model.ImgUrl,
                FullName = model.FullName,
                Designation = model.Designation,
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                UserRole = model.UserRole,
                UpdatedById = model.CreatedAndUpdatedById,
            };
            var user = await _userRepository.UpdateUserAsync(entity);
            return Ok(user);
        }
        [HttpPut("UpdateProfilePicture{id}")]
        public async Task<ActionResult<User>> UpdateProfilePicture(int id, [FromBody] UserDTO model)
        {
            var entity = new User
            {
                Id = id,
                ImgUrl = model.ImgUrl
            };
            var user = await _userRepository.UpdateProfilePicture(entity);
            return Ok(user);
        }

        [HttpPut("RemoveProfilePicture{id}")]
        public async Task<ActionResult<User>> RemoveProfilePicture(int id, [FromBody] UserDTO model)
        {
            var entity = new User
            {
                Id = id
            };
            var user = await _userRepository.RemoveProfilePicture(entity);
            return Ok(user);
        }

        [HttpPost("StatusChange")]
        public async Task<ActionResult<User>> StatusChange([FromBody] UserDTO model)
        {
            var entity = new User
            {
                Id = model.Id ?? 0,
                FullName = model.FullName,
                Designation = model.Designation,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ImgUrl = model.ImgUrl,
                UserRole = model.UserRole,
                IsActive = model.IsActive ?? 0,
            };
            var user = await _userRepository.UpdateUserAsync(entity);
            return Ok(user);
        }
        [HttpPost("Add-UsersHospital")]
        public async Task<ActionResult<UsersHospital>> Post(List<UsersHospital> model)
        {
            var user = await _userHospitalRepository.AddUsersHospitalAsync(model);
            return Ok(user);
        }
        [HttpPut("Update-UsersHospital")]
        public async Task<ActionResult> Post(UsersHospital model)
        {
            await _userHospitalRepository.UpdateUsersHospitalAsync(model);
            return Ok();
        }
        [HttpPost("Add-UsersDiagnosticCode")]
        public async Task<ActionResult<UsersDiagnosticCode>> Post(List<UsersDiagnosticCode> model)
        {
            var user = await _userDiagnosticCodeRepository.AddUsersDiagnosticCodeAsync(model);
            return Ok(user);
        }
        [HttpPost("Add-UsersChargeCode")]
        public async Task<ActionResult<UsersChargeCode>> Post(List<UsersChargeCode> model)
        {
            var user = await _userChargeCodeRepository.AddUsersChargeCodeAsync(model);
            return Ok(user);
        }
    }
}
