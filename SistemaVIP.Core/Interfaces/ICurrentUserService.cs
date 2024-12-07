using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserId();
        bool IsAuthenticated();
        bool IsInRole(string role);
    }
}
