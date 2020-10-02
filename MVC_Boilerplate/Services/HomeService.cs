using Microsoft.EntityFrameworkCore;
using MVC_Boilerplate.Context;
using MVC_Boilerplate.Models.Db.Home;
using MVC_Boilerplate.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Boilerplate.Services
{
    public class HomeService : IHomeService
    {
        private readonly DBContext _context;

        public HomeService(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(HomeModel home)
        {
            await _context.Homes.AddAsync(home);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var home = await _context.Homes.SingleOrDefaultAsync(b => b.Id == id);

            if (home == null)
            {
                return false;
            }

            _context.Homes.Remove(home);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<HomeModel> Get(int id)
        {
            return await _context.Homes.SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<HomeModel>> GetAll()
        {
            return await _context.Homes.ToListAsync();
        }

        public async Task<bool> Update(HomeModel home)
        {
            _context.Homes.Update(home);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
