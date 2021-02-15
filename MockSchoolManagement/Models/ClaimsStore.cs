using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    public class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role","Create Rold"),
            new Claim("Edit Role","Edit Rold"),
            new Claim("Delete Role","Delete Rold"),
            new Claim("EditStudent","EditStudent")
        };
    }
}
