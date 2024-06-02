using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace ReturnManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userrepo;
        private readonly IRepository<int, UserDetail> _userdetailrepo;
        private readonly ITokenService _tokenService;
        public UserService(IRepository<int,User> userrepo, IRepository<int, UserDetail> userdetail, ITokenService tokenservice)
        {
            _userrepo = userrepo;
            _userdetailrepo = userdetail;
            _tokenService = tokenservice;
        }
        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<LoginReturnDTO> Login(UserLoginDTO loginDTO)
        {
            var udb = (await _userdetailrepo.FindAll(ud => ud.Username == loginDTO.Username));
            if(udb == null)
            {
                throw new UnauthorizedUserException("Invalid username or password");
            }
            var userDB = udb.FirstOrDefault();
            HMACSHA512 hMACSHA = new HMACSHA512(userDB.PasswordHashKey);
            var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            bool isPasswordSame = ComparePassword(encrypterPass, userDB.Password);
            if (isPasswordSame)
            {
                    var user = await _userrepo.Get(userDB.UserId);
                    if (userDB.Status == "Active")
                    {
                        LoginReturnDTO loginReturnDTO = MapUserToLoginReturn(user, userDB);
                        return loginReturnDTO;
                    }

                    throw new UserNotActiveException("Your account is not activated");
            }
            throw new UnauthorizedUserException("Invalid username or password");
        }

        public async Task<RegisterReturnDTO> Register(RegisterUserDTO userDTO)
        {
            User user = null;
            UserDetail userdetails = null;
            try
            {
                var userfound = await _userdetailrepo.FindAll(ud => ud.Username == userDTO.Username);
                if (userfound != null)
                {
                    throw new UsernameAlreadyExistException("Username Already Exist");
                }

                var emailfound = await _userrepo.FindAll(u => u.Email == userDTO.Email);
                if (emailfound != null)
                {
                    throw new UserAlreadyExistException("User Account Already Exists, Please Login!");
                }
            }
            catch (Exception ex)
            {
                if (ex is not ObjectsNotFoundException)
                    throw ex;
            }
            user = MapUserDTOToUser(userDTO);
            userdetails = MapUserDTOToUserDetail(userDTO);
            user = await _userrepo.Add(user);
            userdetails.UserId = user.Id;
            userdetails = await _userdetailrepo.Add(userdetails);
            RegisterReturnDTO registerReturnDTO = MapRegisterReturnDTO(user, userdetails);
            return registerReturnDTO;
        }

        private RegisterReturnDTO MapRegisterReturnDTO(User user, UserDetail userdetails)
        {
            RegisterReturnDTO returnDTO = new RegisterReturnDTO()
            {
                UserId = user.Id,
                Username = userdetails.Username,
                Role = user.Role
            };
            return returnDTO;
        }

        public async Task<string> UpdateUserStatus(UserUpdateStatusDTO userUpdateStatusDTO)
        {
            UserDetail ud = await _userdetailrepo.Get(userUpdateStatusDTO.UserId);
            if (ud == null)
            {
                throw new NoUserFoundException("No User Found");
            }
            ud.Status = userUpdateStatusDTO.Status;
            await _userdetailrepo.Update(ud);
            return "User Status Successfully Updated";
                
        }

        private User MapUserDTOToUser(RegisterUserDTO userDTO)
        {
            User user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Phone = userDTO.Phone,
                Address = userDTO.Address,
                Role = "User",
            };
            return user;
        }

        private LoginReturnDTO MapUserToLoginReturn(User user, UserDetail userDB)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.UserID = user.Id;
            returnDTO.Username = userDB.Username;
            returnDTO.Role = user.Role ?? "User";
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }

        private UserDetail MapUserDTOToUserDetail(RegisterUserDTO userDTO)
        {
            UserDetail userdetail = new UserDetail();
            userdetail.Status = "Disabled";
            userdetail.Username = userDTO.Username;
            HMACSHA512 hMACSHA = new HMACSHA512();
            userdetail.PasswordHashKey = hMACSHA.Key;
            userdetail.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password));
            return userdetail;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var result = await _userrepo.GetAll();
            if (result == null)
                throw new ObjectsNotFoundException("Users Not Found");
            return result;
        }

        public async Task<string> UpdateUserRole(int userId, string role)
        {
            User user = await _userrepo.Get(userId);
            if (user == null)
            {
                throw new NoUserFoundException("No User Found");
            }
            user.Role = role;
            await _userrepo.Update(user);
            return $"{user.Name} is now {role}";
        }
    }
}

