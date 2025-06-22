using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AppointmentsSlot
{
    public int SlotId { get; set; }

    public int ProviderKey { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly SlotStart { get; set; }

    public TimeOnly SlotEnd { get; set; }

    public int BranchId { get; set; }

    public bool IsBooked { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ServiceProvider ProviderKeyNavigation { get; set; } = null!;
}
