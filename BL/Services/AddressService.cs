using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using BLL.Models;
using DAL.Services;
namespace BLL.Services
{
    internal class AddressService
    {
        private readonly PatientsManagement _patientManagement;
        private readonly AddressManagement _addressManagement;

        public AddressService(PatientsManagement patientManagement, AddressManagement addressManagement)
        {
            _patientManagement = patientManagement;
            _addressManagement = addressManagement;
        }
       
        public static async Task<int> GetOrAddAddressAsync(Address a)
        {
            var existing = await _addressManagement.GetAddressById(a.AddressId);
            if (existing != null)
                return existing.AddressId;
            return await _addressManagement.AddAddress(a);
        }



    }
}
