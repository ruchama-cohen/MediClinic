using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class ServiceProvider
{
    public int ProviderId { get; set; }

    public string Name { get; set; } = null!;

    public int ServiceId { get; set; }

    public int BranchToProviderId { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? MeetingTime { get; set; }

    public virtual ICollection<AppointmentsSlot> AppointmentsSlots { get; set; } = new List<AppointmentsSlot>();

    public virtual BranchToServiceProvider BranchToProvider { get; set; } = null!;

    public virtual ICollection<BranchToServiceProvider> BranchToServiceProviders { get; set; } = new List<BranchToServiceProvider>();

    public virtual ClinicService Service { get; set; } = null!;

    public virtual ICollection<WorkHour> WorkHours { get; set; } = new List<WorkHour>();
}
