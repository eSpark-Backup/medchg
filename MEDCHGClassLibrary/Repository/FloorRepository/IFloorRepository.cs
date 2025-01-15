using MEDCHGClassLibrary.Model;
using MEDCHGClassLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.FloorRepository
{
    public interface IFloorRepository
    {
        Task<List<Floor>> GetAllFloors();
        Task<ResponseMessage> AddFloor(Floor model);
        Task<ResponseMessage> UpdateFloor(Floor model);
        Task<ResponseMessage> DeleteFloor(int FloorId, int UpdatedById);
    }
}
