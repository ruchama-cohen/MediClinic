using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string BranchName { get; set; } = null!;

    public int AddressId { get; set; }

    public string Phone { get; set; } = null!;

    public TimeOnly OpeningHour { get; set; }

    public TimeOnly ClosingHour { get; set; }

    public int BranchManagerId { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<AppointmentsSlot> AppointmentsSlots { get; set; } = new List<AppointmentsSlot>();

    public virtual ClinicService BranchManager { get; set; } = null!;

    public virtual ICollection<ServiceProvider> ServiceProviders { get; set; } = new List<ServiceProvider>();

    public virtual ICollection<WorkHour> WorkHours { get; set; } = new List<WorkHour>();
}
