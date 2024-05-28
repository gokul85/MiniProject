using ReturnManagementSystem.Models.DTOs;
using ReturnManagementSystem.Models;

namespace ReturnManagementSystem.Interfaces
{
    public interface IUserService
    {
        public Task<LoginReturnDTO> Login(UserLoginDTO loginDTO);
        public Task<RegisterReturnDTO> Register(RegisterUserDTO employeeDTO);
        public Task<string> UpdateUserStatus(UserUpdateStatusDTO userUpdateStatusDTO);
    }
}
