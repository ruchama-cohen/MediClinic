using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    internal interface IBranchToServiceProviderManagement
    {
        Task AddBranchToServiceProvider(BranchToServiceProvider branchToServiceProvider);
        Task<bool> DeleteBranchToServiceProvider(int id);
    }
}
