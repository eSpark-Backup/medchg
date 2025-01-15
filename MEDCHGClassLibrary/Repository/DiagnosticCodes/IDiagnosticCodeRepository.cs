using MEDCHGClassLibrary.Model;
using System.Linq.Expressions;

namespace MEDCHGClassLibrary.Repository.DiagnosticCodes
{
    public interface IDiagnosticCodeRepository
    {
        Task AddDiagnosticCodeAsync(List<DiagnosticCode> model);
        Task<DiagnosticCode> UpdateDiagnosticCodeAsync(DiagnosticCode model);
        Task<bool> DeleteDiagnosticCodeAsync(int DiagnosticCodeId,string UserId);
        Task<List<DiagnosticCode>> GetAllDiagnosticCodeAsync();
        Task<DiagnosticCode> GetDiagnosticCodeById(Expression<Func<DiagnosticCode, bool>> where);
        Task<DiagnosticCode> GetDiagnosticCodeByIdAsync(DiagnosticCode DiagnosticCodeId);
        Task UpdateDiagnosticCodeByIdAsync(int id, bool fav,int UpdatedById);
    }
}
