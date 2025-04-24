using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public int? HouseNumber { get; set; }

    public string? PostalCode { get; set; }

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
