using MEDCHGClassLibrary.Model;

namespace MEDCHGClassLibrary.Repository.RoomRepository
{
	public interface IRoomRepository
	{
		Task<List<Room>> GetAllRooms();
		Task<List<string>> GetAllOccupiedRooms(int HospitalId);
		Task<bool> DeleteRoom(int Id, int UpdatedById);
	}
}
