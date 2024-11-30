using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Enums
{
    public static class UserRoles
    {
        public const string SUPER_ADMIN = "SUPER_ADMIN";
        public const string ADMIN = "ADMIN";
        public const string PRESENTADOR = "PRESENTADOR";

        public static readonly IReadOnlyList<string> AllRoles = new[]
        {
            SUPER_ADMIN,
            ADMIN,
            PRESENTADOR
        };
    }
}