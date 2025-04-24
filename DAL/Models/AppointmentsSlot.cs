using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AppointmentsSlot
{
    public int SlotId { get; set; }

    public int ProviderId { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly SlotStart { get; set; }

    public TimeOnly SlotEnd { get; set; }

    public int BranchId { get; set; }

    public bool IsBooked { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Branch Branch { get; set; } = null!;

    public virtual ServiceProvider Provider { get; set; } = null!;
}
