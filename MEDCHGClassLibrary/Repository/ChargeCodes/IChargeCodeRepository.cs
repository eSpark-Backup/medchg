using MEDCHGClassLibrary.Model;
using System.Linq.Expressions;

namespace MEDCHGClassLibrary.Repository.ChargeCodes
{
    public interface IChargeCodeRepository
    {
        Task AddChargeCodeAsync(List<ChargeCode> model);
        Task<ChargeCode> UpdateChargeCodeAsync(ChargeCode model);
        Task<bool> DeleteChargeCodeAsync(int ChargeCodeId,string UserId);
        Task<List<ChargeCode>> GetAllChargeCodeAsync();
        Task<ChargeCode> GetChargeCodeById(Expression<Func<ChargeCode, bool>> where);
        Task<ChargeCode> GetChargeCodeByIdAsync(ChargeCode ChargeCodeId);
        Task UpdateChargeCodeByIdAsync(int id, bool fav, int UpdatedById);
    }
}
