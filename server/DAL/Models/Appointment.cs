using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int SlotId { get; set; }

    public int PatientKey { get; set; }

    public virtual Patient PatientKeyNavigation { get; set; } = null!;

    public virtual AppointmentsSlot Slot { get; set; } = null!;
}
