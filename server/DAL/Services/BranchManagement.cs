using DAL.API;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DAL.Services
{
    public class BranchManagement : IBranchManagement
    {
        private readonly DB_Manager _context;

        public BranchManagement(DB_Manager context)
        {
            _context = context;
        }
        public async Task AddBranch(Branch branch)
        {
            await _context.Set<Branch>().AddAsync(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBranch(int branchId)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.BranchId == branchId);
            if (branch == null)
                return false;
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Branch>> GetAllBranches()
        {
            return await _context.Branches.ToListAsync();
        }

        public async Task<List<Branch>> GetBranchesByCityID(int cityID)
        {
            return await _context.Branches
                .Where(b => b.Address.City.CityId==cityID)
                .ToListAsync();
        }

        public async Task<List<Branch>> GetBranchesByServiceProviderKey(int doctorKey)
        {
            return await _context.Branches
                  .Where(b => b.ServiceProviders.Any(sp => sp.ProviderKey== doctorKey))
                  .ToListAsync();

        }


        public async Task<bool> UpdateBranchDetails(Branch updatedBranch)
        {
            var branch = await _context.Branches.FindAsync(updatedBranch.BranchId);

            if (branch == null)
                return false;

            _context.Entry(branch).CurrentValues.SetValues(updatedBranch);

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
