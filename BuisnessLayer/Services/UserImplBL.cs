using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using BuisnessLayer.InterFaces;
using RepositoryLayer.Services;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;
namespace BuisnessLayer.Services
{
    public class UserImplBL : IUserBL
    {
        public IUserRL _userRl;
        public UserImplBL(IUserRL userRl)
        {
            _userRl = userRl;
        }

        public async Task<ResponseDTO<string>> RegisterAsync(RegisterDTO _registerRequest)
        {
            try
            {
                var result = await _userRl.RegisterAsync(_registerRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> DeleteAsync(string email)
        {
            try
            {
                var result = await _userRl.DeleteAsync(email);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> LoginAsync(LoginDTO loginRequest)
        {
            try
            {
                var result = await _userRl.LoginAsync(loginRequest);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> GetAllUserAsync()
        {
            try
            {
                var result = await _userRl.GetAllUserAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<ResponseDTO<string>> ChangePasswordAsync(string email, string oldPass, string newPass)
        {
            try
            {
                var result = _userRl.ChangePasswordAsync(email, oldPass, newPass);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> ForgetPasswordAsync(string email)
        {
            try
            {
                var result = await _userRl.ForgetPasswordAsync(email);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO<string>> ResetPassword(string email, string newPassword)
        {
            try
            {
                var result = await _userRl.ResetPassword(email, newPassword);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
