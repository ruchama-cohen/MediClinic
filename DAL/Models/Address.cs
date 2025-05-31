using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int CityId { get; set; }

    public int StreetId { get; set; }

    public int HouseNumber { get; set; }

    public string PostalCode { get; set; } = null!;

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual Street Street { get; set; } = null!;

}
