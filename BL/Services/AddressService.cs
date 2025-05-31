using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using BLL.Models;
using DAL.API;

namespace BLL.Services
{
    internal class AddressService
    {
        private readonly IPatientsManagement _patientManagement;
        private readonly IAddressManagement _addressManagement;

        public AddressService(IPatientsManagement patientManagement, IAddressManagement addressManagement)
        {
            _patientManagement = patientManagement;
            _addressManagement = addressManagement;
        }

        public async Task<int> GetOrAddAddressAsync(Address address)
        {
            var existing = await _addressManagement.GetAddressById(address.AddressId);
            if (existing != null)
                return existing.AddressId;
            return await _addressManagement.AddAddress(address);
        }
    }
}