using MEDCHGClassLibrary.Services;
using MEDCHGClassLibrary.ViewModels.Building;

namespace MEDCHGClassLibrary.Repository.BuildingRepository
{
    public interface IBuildingRepository
    {
        Task<ResponseMessage> AddBuilding(Model.Building model);
        Task<ResponseMessage> UpdateBuilding(Model.Building model);
        Task<List<Model.Building>> GetAllBuildings();
        Task<Model.Building> GetBuildingById(int Id);
        Task<BuildingPatientListVm> GetBuildingPatients(int buildingId);
        Task<bool> DeleteBuilding(int buildingId);
    }
}
