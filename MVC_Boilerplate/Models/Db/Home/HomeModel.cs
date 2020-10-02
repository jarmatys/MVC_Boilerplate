using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Boilerplate.Models.Db.Home
{
    public class HomeModel
    {
        [Key]
        public int Id { get; set; }
    }
}
