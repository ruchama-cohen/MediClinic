using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class City
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
