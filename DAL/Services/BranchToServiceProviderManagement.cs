using DAL.API;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class BranchToServiceProviderManagement : IBranchToServiceProviderManagement
    {
        private readonly DB_Manager _context;

        public BranchToServiceProviderManagement(DB_Manager context)
        {
            _context = context;
        }
        public async Task AddBranchToServiceProvider(BranchToServiceProvider branchToServiceProvider)
        {
            await _context.BranchToServiceProviders.AddAsync(branchToServiceProvider);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBranchToServiceProvider(int id)
        {
            var branchToServiceProvider = await _context.BranchToServiceProviders.FindAsync(id);
            if (branchToServiceProvider != null)
            {
                _context.BranchToServiceProviders.Remove(branchToServiceProvider);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
