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

        Task<List<Branch>> GetBranchesByCity(string city);

        Task<List<Branch>> GetBranchesByDoctor(int doctorKey);

        Task<List<Branch>> GetAllBranches();
    }
}
