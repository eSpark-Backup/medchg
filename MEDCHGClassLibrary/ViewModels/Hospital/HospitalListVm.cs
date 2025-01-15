using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.ViewModels.Hospital;

public class HospitalListVm
{
    public HospitalListVm()
    {
        Items = new List<HospitalListItemVm>();
    }
    public int ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public List<HospitalListItemVm> Items { get; set; }
}
public class HospitalListItemVm
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? PatientVisitCount { get; set; }
    public bool? Status { get; set; }
}