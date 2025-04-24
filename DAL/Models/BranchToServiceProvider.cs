using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class BranchToServiceProvider
{
    public int Id { get; set; }

    public int ServicProviderId { get; set; }

    public int BranchId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ServiceProvider ServicProvider { get; set; } = null!;

    public virtual ICollection<ServiceProvider> ServiceProviders { get; set; } = new List<ServiceProvider>();
}
