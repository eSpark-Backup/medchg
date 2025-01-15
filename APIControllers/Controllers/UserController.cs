using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Repository.User;
using MEDCHGClassLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MEDCHGClassLibrary.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIControllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepository _userRepository) : ControllerBase
    {
        // GET: api/<UserController>
        [HttpPost("SaveUser")]
        [AllowAnonymous]

        public async Task<IActionResult> SaveUser(UserDTO model)
        {
            ResponseApi resp = new ResponseApi();
            resp.IsSuccess = true;
            try
            {

                if (string.IsNullOrEmpty(model.FullName))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Name is required.";
                }
                else if (string.IsNullOrEmpty(model.UserName))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Username is required.";
                }
                else if (string.IsNullOrEmpty(model.Email))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Email is required.";
                }
                else if (string.IsNullOrEmpty(model.UserRole))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Role is required.";
                }
                else if (string.IsNullOrEmpty(model.Password))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Password is required.";
                }
                else if (Convert.ToInt32(model.ProviderId) == 0)
                {
                    resp.IsSuccess = false;
                    resp.Message = "Provider is required.";
                }
                else if (string.IsNullOrEmpty(model.Designation))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Designation is required.";
                }
                else
                {
                    if (model.UserRole is not "Editor Access" && model.UserRole is not "Operator Access")
                    {
                        model.UserName = model.UserName.ToLower() + "." + model.ProviderName;
                    }
                    var user = await _userRepository.SaveUser(model);
                    if (user != null)
                    {
                        resp.IsSuccess = true;
                        resp.Data = user;
                        resp.Message = "User saved successfully.";
                    }
                }
            }
            catch (Exception)
            {
                resp.IsSuccess = false;
                resp.Message = "Failed: something went wrong.";
            }
            return Ok(resp);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            ResponseApi resp = new ResponseApi();
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Username is required.";
                }
                else if (string.IsNullOrEmpty(model.ProviderName))
                {
                    resp.IsSuccess = false;
                    resp.Message = "ProviderName is required.";
                }
                else if (string.IsNullOrEmpty(model.Password))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Password is required.";
                }
                else
                {
                    var user = await _userRepository.GetUserByUsername(model.UserName, model.ProviderName);
                    if (user != null)
                    {
                        if (user.IsActive == 1)
                        {
                            if (user.Password == EncryptDecrypt.HashStringWithSalt(model.Password))
                            {
                                resp.IsSuccess = true;
                                resp.Data = user;
                                resp.Message = "Login Successfully.";
                            }
                            else
                            {
                                resp.IsSuccess = false;
                                resp.Message = "Password is incorrect.";
                            }
                        }
                        else
                        {
                            resp.IsSuccess = false;
                            resp.Message = "Account is in-active, contact admin.";
                        }
                    }
                    else
                    {
                        resp.IsSuccess = false;
                        resp.Message = "User not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = "Failed: something went wrong.";
            }
            return Ok(resp);
        }

        [HttpPost("LoginAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginDTO model)
        {
            ResponseApi resp = new ResponseApi();
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Username is required.";
                }
                else if (string.IsNullOrEmpty(model.Password))
                {
                    resp.IsSuccess = false;
                    resp.Message = "Password is required.";
                }
                else
                {
                    var user = await _userRepository.GetAdminByLogin(model.UserName);
                    if (user != null)
                    {
                        if (user.IsActive == 1)
                        {
                            if (user.Password == EncryptDecrypt.HashStringWithSalt(model.Password))
                            {
                                resp.IsSuccess = true;
                                resp.Data = user;
                                resp.Message = "Login Successfully.";
                            }
                            else
                            {
                                resp.IsSuccess = false;
                                resp.Message = "Password is incorrect.";
                            }
                        }
                        else
                        {
                            resp.IsSuccess = false;
                            resp.Message = "Account is in-active, contact admin.";
                        }
                    }
                    else
                    {
                        resp.IsSuccess = false;
                        resp.Message = "User not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                resp.IsSuccess = false;
                resp.Message = "Failed: something went wrong.";
            }
            return Ok(resp);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
