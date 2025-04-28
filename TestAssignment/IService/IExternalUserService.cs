using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.Model;

namespace TestAssignment.IService
{
    public interface IExternalUserService
    {
        Task<UserData> GetUserByIdAsync(int userId);
        Task<List<UserData>> GetAllUsersAsync();
    }

}
