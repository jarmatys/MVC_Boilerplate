using MVC_Boilerplate.Models.Db.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Boilerplate.Services.Interfaces
{
    public interface IHomeService
    {
        Task<bool> Create(HomeModel home);
        Task<HomeModel> Get(int id);
        Task<List<HomeModel>> GetAll();
        Task<bool> Update(HomeModel home);
        Task<bool> Delete(int id);
    }
}
