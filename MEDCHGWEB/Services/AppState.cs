using MEDCHGClassLibrary.DTO;
using MEDCHGClassLibrary.Model;
using MEDCHGWEB.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

namespace MEDCHGWEB.Services
{
    public class AppState
    {
        public CurrentUserModel? currentUser;
        public PatientVisitHistory PatientVisitHistory { get; set; } = new();
        public Patient PatientInfo { get; set; } = new();
        public List<Patient> PatientInfoList { get; set; } = new();
        public PatientDTO PatientDTO { get; set; } = new();
        public List<PatientDTO> PatientDTOList { get; set; } = new();

        private int _BillingPVTab = 1;
        public int BillingPVTab
        {
            get => _BillingPVTab;
            set
            {
                if (_BillingPVTab != value)
                {
                    _BillingPVTab = value;
                    NotifyBillingPVTabChanged();
                }
            }
        }

        public event Action OnBillingPVTabChanged;

        private void NotifyBillingPVTabChanged() => OnBillingPVTabChanged?.Invoke();
    }
}
