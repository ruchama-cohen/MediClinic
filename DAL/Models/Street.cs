using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Street
{
    public int StreetId { get; set; }

    public string Name { get; set; } = null!;

    public int CityId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual City City { get; set; } = null!;
}
