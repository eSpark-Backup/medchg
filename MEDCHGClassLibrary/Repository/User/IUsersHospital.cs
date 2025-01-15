using MEDCHGClassLibrary.Model;

namespace MEDCHGClassLibrary.Repository.User
{
    public interface IUsersHospital
    {
        Task<List<UsersHospital>> AddUsersHospitalAsync(List<UsersHospital> model);
        Task UpdateUsersHospitalAsync(UsersHospital model);
        Task<UsersHospital> DeleteUsersHospitalAsync(int UsersHospitalId);
        Task<List<UsersHospital>> GetAllUsersHospitalAsync();
        Task<List<UsersHospital>> GetUsersHospitalByIdAsync(int UsersHospitalId);
    }
}
