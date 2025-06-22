using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IWorkHourManagement
    {
        Task AddWorkHour(WorkHour workHour);
        Task<bool> UpdateWorkHourDetails(WorkHour workHour);
        Task<bool> DeleteWorkHour(int branchId);
    }
}
