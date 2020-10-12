using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Boilerplate.Models.View.Account
{
    public class RoleToUserView
    {
        [Require]
        public string userId { get; set; }

        [Require]
        public string roleId { get; set; }
    }
}
