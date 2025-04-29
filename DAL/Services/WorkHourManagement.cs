using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    internal class WorkHourManagement: IWorkHourManagement
    {
        private readonly DB_Manager _context;

        public WorkHourManagement()
        {
            _context = new DB_Manager();
        }
        public async Task AddWorkHour(WorkHour workHour)
        {
            await _context.WorkHours.AddAsync(workHour);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteWorkHour(int branchId)
        {
            var branch = await _context.WorkHours.FirstOrDefaultAsync(b => b.BranchId == branchId);
            if (branch == null)
                return false;
            _context.WorkHours.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

 
        public async Task<bool> UpdateWorkHourDetails(WorkHour workHour)
        {
            var workHourN = await _context.WorkHours.FindAsync(workHour.WorkHourId);

            if (workHourN == null)
                throw new Exception("Branch not found");

            _context.Entry(workHourN).CurrentValues.SetValues(workHour);

            await _context.SaveChangesAsync();
            return true;
        }

       
    }
}
