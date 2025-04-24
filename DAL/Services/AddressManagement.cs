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
    internal class AddressManagement : IAddressManagement
    {
        private readonly DB_Manager _context;

        public AddressManagement()
        {
            _context = new DB_Manager();
        }
        public async Task AddAddress(Address address)
        {
            await _context.Addresses.AddAsync(address);

            await _context.SaveChangesAsync();
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

        public async Task<string?> GetCityById(int id)
        {
            return await _context.Addresses
                .Where(x => x.AddressId == id)
                .Select(x => x.City)
                .FirstOrDefaultAsync();
        }


        public async Task<Address?> SearchAddress(Address address)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a =>
                a.City == address.City &&
                a.Street == address.Street &&
                a.HouseNumber == address.HouseNumber &&
                a.PostalCode == address.PostalCode);
        }
    }
}
