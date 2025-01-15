using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.ViewModels.Building;
using MEDCHGClassLibrary.ViewModels.Hospital;

namespace MEDCHGClassLibrary.Repository.Hospital
{
    public interface IHospitalRepository
    {
        Task<ResponseMessage> AddHospital(Model.Hospital model);
        Task<ResponseMessage> CheckHospitalNameUnique(Model.Hospital model);
        Task<CountVM> GetHospitalById(int id);
        Task<Model.Hospital> GetAllHospitalsWithThereForeignKeys(int id);
        Task<HospitalListVm> GetHospitalWithProvider(int providerId, int userId);
        Task<HospitalBuildingListVm> GetHospitalBuildings(int hospitalId);
        Task<List<Model.Hospital>> GetAllHospitals();
        Task<Model.Hospital> GetHospitalBuildingsRooms(int HospitalId);
        Task<bool> DeleteHospital(int hospitalId);
    }
}
