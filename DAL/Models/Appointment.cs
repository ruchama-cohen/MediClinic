using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public int ProviderId { get; set; }

    public int PatientId { get; set; }

    public int StatusId { get; set; }

    public int BranchId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ServiceProvider Provider { get; set; } = null!;

    public virtual AppointmentStatus Status { get; set; } = null!;
}
