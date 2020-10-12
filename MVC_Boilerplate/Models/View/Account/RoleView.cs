using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Boilerplate.Models.View.Account
{
    public class RoleView
    {
        [Required]
        public string Name { get; set; }
    }
}
