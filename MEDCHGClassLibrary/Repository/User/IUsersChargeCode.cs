using MEDCHGClassLibrary.Model;

namespace MEDCHGClassLibrary.Repository.User
{
    public interface IUsersChargeCode
    {
        Task<List<UsersChargeCode>> AddUsersChargeCodeAsync(List<UsersChargeCode> model);
        Task<UsersChargeCode> UpdateUsersChargeCodeAsync(UsersChargeCode model);
        Task<UsersChargeCode> DeleteUsersChargeCodeAsync(int UsersChargeCodeId);
        Task<List<UsersChargeCode>> GetAllUsersChargeCodeAsync();
        Task<List<UsersChargeCode>> GetUsersChargeCodeByIdAsync(int UsersChargeCodeId);
    }
}
