using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.DTO;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRL
    {
        Task<ResponseDTO<string>> RegisterAsync(RegisterDTO _registerRequest);
        Task<ResponseDTO<string>> DeleteAsync(string email);
        Task<ResponseDTO<string>> LoginAsync(LoginDTO loginRequest);

        Task<ResponseDTO<string>> GetAllUserAsync();
        Task<ResponseDTO<string>> ChangePasswordAsync(string email,string oldPass,string newPass);
        Task<ResponseDTO<string>> ForgetPasswordAsync(string email);
        Task<ResponseDTO<string>> ResetPassword(string email,string newPassword);

    }
}
