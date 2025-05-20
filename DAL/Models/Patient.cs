using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Patient
{
    public int PatientKey { get; set; }

    public string PatientId { get; set; } = null!;

    public string PatientName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string PatientPassword { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int AddressId { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
