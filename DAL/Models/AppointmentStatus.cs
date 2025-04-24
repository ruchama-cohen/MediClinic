using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class AppointmentStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
