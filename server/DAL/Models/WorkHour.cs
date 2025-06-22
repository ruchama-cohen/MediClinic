using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class WorkHour
{
    public int WorkHourId { get; set; }

    public int BranchId { get; set; }

    public int Weekday { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int ProviderKey { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ServiceProvider ProviderKeyNavigation { get; set; } = null!;
}
