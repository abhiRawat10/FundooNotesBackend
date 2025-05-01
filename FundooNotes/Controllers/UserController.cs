using BuisnessLayer.InterFaces;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO _registerRequest)
        {
            try
            {
                logger.Info("RegisterAsync called with email: ", _registerRequest.email);
                var result = await _userBL.RegisterAsync(_registerRequest);

                if (result != null)
                {
                    logger.Info("User registered successfully: ", _registerRequest.email);
                    return Ok(result);
                }

                logger.Warn("User registration failed: ", _registerRequest.email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in RegisterAsync for email: {0}", _registerRequest.email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAsync(string email)
        {
            try
            {
                logger.Info("DeleteAsync called for email: {0}", email);
                var result = await _userBL.DeleteAsync(email);

                if (result != null)
                {
                    logger.Info("User deleted successfully: {0}", email);
                    return Ok(result);
                }

                logger.Warn("User deletion failed: {0}", email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in DeleteAsync for email: {0}", email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDTO loginRequest)
        {
            try
            {
                logger.Info("LoginAsync called for email: {0}", loginRequest.email);
                var result = await _userBL.LoginAsync(loginRequest);

                if (result.success)
                {
                    logger.Info("Login successful for email: {0}", loginRequest.email);
                    return Ok(result);
                }

                logger.Warn("Login failed for email: {0}", loginRequest.email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in LoginAsync for email: {0}", loginRequest.email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                logger.Info("GetAllUserAsync called");
                var result = await _userBL.GetAllUserAsync();

                if (result.success)
                {
                    logger.Info("GetAllUserAsync successful");
                    return Ok(result);
                }

                logger.Warn("GetAllUserAsync failed");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in GetAllUserAsync");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePasswordAsync(string oldPass, string newPass)
        {
            var email = User.FindFirst("Email")?.Value;
            logger.Info("ChangePasswordAsync called for email: {0}", email);

            if (email == null)
            {
                logger.Warn("ChangePasswordAsync failed: Email not found in token");
                return BadRequest(new { Success = false, message = "Email not found in token" });
            }

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass))
                {
                    logger.Warn("ChangePasswordAsync failed due to empty values");
                    return BadRequest(new { Success = false, message = "email or password is empty" });
                }

                var result = await _userBL.ChangePasswordAsync(email, oldPass, newPass);

                if (result.success)
                {
                    logger.Info("Password changed successfully for email: {0}", email);
                    return Ok(result);
                }

                logger.Warn("ChangePasswordAsync failed for email: {0}", email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ChangePasswordAsync for email: {0}", email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            logger.Info("ForgetPasswordAsync called for email: {0}", email);

            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    logger.Warn("ForgetPasswordAsync failed: Email is empty");
                    return BadRequest(new { Success = false, message = "email is empty" });
                }

                var result = await _userBL.ForgetPasswordAsync(email);

                if (result.success)
                {
                    logger.Info("ForgetPasswordAsync succeeded for email: {0}", email);
                    return Ok(result);
                }

                logger.Warn("ForgetPasswordAsync failed for email: {0}", email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ForgetPasswordAsync for email: {0}", email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string newPassword)
        {
            var email = User.FindFirst("Email")?.Value;
            try
            {
                
                logger.Info("ResetPassword called for email: {0}", email);
                if (email == null)
                {
                    logger.Warn("ResetPassword failed: Email not found in token");
                    return BadRequest(new { Success = false, message = "Email not found in token" });
                }

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
                {
                    logger.Warn("ResetPassword failed due to empty values");
                    return BadRequest(new { Success = false, message = "email or password is empty" });
                }
                var result = await _userBL.ResetPassword(email, newPassword);
                if (result.success)
                {
                    logger.Info("Password reset successfully for email: {0}", email);
                    return Ok(result);
                }
                logger.Warn("ResetPassword failed for email: {0}", email);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ResetPassword for email: {0}", email);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
