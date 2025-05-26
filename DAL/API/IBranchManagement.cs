using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IBranchManagement
    {
        Task<bool> UpdateBranchDetails(Branch branch);

        Task<bool> DeleteBranch(int branchId);

        Task AddBranch(Branch branch);

        Task<List<Branch>> GetBranchesByCityID(int cityID);

        Task<List<Branch>> GetBranchesByServiceProviderKey(int doctorKey);

        Task<List<Branch>> GetAllBranches();
    }
}
