using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Enum
{
    public static class UserRoles
    {
        public static string Superadmin = "Superadmin";
        public static string Admin = "Admin";
        public static string Doctor = "Doctor";
        public static string User = "User";
        public static string EditorAccess = "Editor Access";
        public static string OperatorAccess = "Operator Access";
        public static List<string> GetAllRoles()
        {
            return new List<string>
            {
                Superadmin,
                Admin,
                Doctor,
                User,
                EditorAccess,
                OperatorAccess
            };
        }
    }

}
