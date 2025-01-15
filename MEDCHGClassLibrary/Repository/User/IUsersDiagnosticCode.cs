using MEDCHGClassLibrary.Model;

namespace MEDCHGClassLibrary.Repository.User
{
    public interface IUsersDiagnosticCode
    {
        Task<List<UsersDiagnosticCode>> AddUsersDiagnosticCodeAsync(List<UsersDiagnosticCode> model);
        Task<UsersDiagnosticCode> UpdateUsersDiagnosticCodeAsync(UsersDiagnosticCode model);
        Task<UsersDiagnosticCode> DeleteUsersDiagnosticCodeAsync(int UsersDiagnosticCodeId);
        Task<List<UsersDiagnosticCode>> GetAllUsersDiagnosticCodeAsync();
        Task<List<UsersDiagnosticCode>> GetUsersDiagnosticCodeByIdAsync(int UsersDiagnosticCodeId);
    }
}
