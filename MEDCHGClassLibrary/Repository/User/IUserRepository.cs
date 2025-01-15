
using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.User
{
    public interface IUserRepository
    {
        Task<Model.User> SaveUser(UserDTO user);
        Task<Model.User> AddUserAsync(UserDTO user);
        Task<List<UserDTO>> List();
        Task<Dictionary<int, string>> GetUsers();  
        Task<Model.User> GetUserById(int id);
        Task<UserDTO> GetUserByLogin(string userName, string providerName);
        Task<UserDTO> GetUserByUsername(string userName, string providerName);
        Task<UserDTO> GetAdminByLogin(string userName);
        Task<Model.User> UpdateUserAsync(Model.User model);
        Task<Model.User> UpdateUserStatusAsync(UserDTO model);
        Task<Model.User> DeleteUserAsync(int UserId);
        Task<List<Model.User>> GetAllUsersAsync();
        Task<List<Model.User>> GetAllUsersByProviderIdAsync(int Id);
        Task<Model.User> GetEmailUser(string UserEmail);
        Task<Model.User> UpdateUserPasswordAsync(byte[] password);
        Task<ResponseMessage> UpdateAllUserAsync(List<Model.User> model);
        Task<Model.User?> GetUserByProviderAndUsername(int providerId, string userName);
        Task<Model.User> UpdateProfilePicture(Model.User model);
        Task<Model.User> RemoveProfilePicture(Model.User model);
    }
}
