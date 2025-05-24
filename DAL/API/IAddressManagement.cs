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
        Task AddAddress(Address address);
        Task<Address?> SearchAddress(Address address);

        Task<string?> GetCityNameById(int id);
        Task<bool> DeleteAddress(int id);

    }
}
