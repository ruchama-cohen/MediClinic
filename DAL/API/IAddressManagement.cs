using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.API
{
    public interface IAddressManagement
    {
        Task<int> AddAddress(Address address);

        Task<Address?> GetAddressById(int id);
        Task<bool> DeleteAddress(int id);

    }
}
