using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class BranchToServiceProvider
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int ServiceProviderId { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ServiceProvider ServiceProvider { get; set; } = null!;
}