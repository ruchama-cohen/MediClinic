using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ClinicService
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual ICollection<ServiceProvider> ServiceProviders { get; set; } = new List<ServiceProvider>();
}
