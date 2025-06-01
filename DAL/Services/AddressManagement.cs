using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace DAL.Services
{

    public class AddressManagement : IAddressManagement
    {
        private readonly DB_Manager _context;

        public AddressManagement(DB_Manager context)
        {
            _context = context;
        }
        public async Task<int> AddAddress(Address address)
        {
            await _context.Addresses.AddAsync(address);

            await _context.SaveChangesAsync();
            return address.AddressId; 
        }
        public async Task<bool> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<string?> GetCityNameById(int id)
        {
            return await _context.Addresses
                .Where(x => x.AddressId == id)
                .Select(x => x.City.Name)
                .FirstOrDefaultAsync();
        }
        public async Task<Address?> GetAddressById(int id)
        {
            return await _context.Addresses
                .Where(x => x.AddressId == id)
               
                .FirstOrDefaultAsync();
        }

     
    }
}
