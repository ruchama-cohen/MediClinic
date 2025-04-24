using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int SlotId { get; set; }

    public int PatientId { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual AppointmentsSlot Slot { get; set; } = null!;
}
