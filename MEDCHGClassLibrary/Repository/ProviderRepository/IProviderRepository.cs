using MEDCHGClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Repository.ProviderRepository
{
    public interface IProviderRepository
    {
        Task<Provider> AddProviderAsync(Provider model);
        Task<Provider> UpdateProviderStatusAsync(Provider model);
        Task<Provider> DeleteProviderAsync(int ProviderId);
        Task<List<Provider>> GetAllProviderAsync();
        Task<Provider> GetAllProviderAsyncLast();
        Task<Provider> GetProviderByIdAsync(int ProviderId);
        Task<List<Model.Hospital>> GetAllHospitalsByProviderId(int id);
        Task<List<DiagnosticCode>> GetAllDiagnosticCodesByProviderId(int id);
        Task<List<ChargeCode>> GetAllChargeCodesByProviderId(int id);
        Task<List<Model.User>> GetAllUsersByProviderId(int id);
        Task<ProviderTimerSetting> GetTimerSettings(int id);
        Task<Provider> GetProviderUsersAsync(int ProviderId);
		Task<ProviderTimerSetting> SaveTimerSettings(ProviderTimerSetting model);
    }
}
