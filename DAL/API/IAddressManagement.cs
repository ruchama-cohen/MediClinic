using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.API
{
    internal interface IAddressManagement
    {
        Task AddAddress(Address address);
        Task<Address?> SearchAddress(Address address);

        Task<string?> GetCityByIdAsync(int id);
        Task<bool> DeleteAddressAsync(int id);

    }
}
