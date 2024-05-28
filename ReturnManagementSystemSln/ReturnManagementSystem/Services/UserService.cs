using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs;
using ReturnManagementSystem.Repositories;
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
            try
            {
                var userDB = await _userdetailrepo.Get(loginDTO.UserId);
                if (userDB == null)
                {
                    throw new UnauthorizedUserException("Invalid username or password");
                }
                HMACSHA512 hMACSHA = new HMACSHA512(userDB.PasswordHashKey);
                var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                bool isPasswordSame = ComparePassword(encrypterPass, userDB.Password);
                if (isPasswordSame)
                {
                    var user = await _userrepo.Get(loginDTO.UserId);
                    if (userDB.Status == "Active")
                    {
                        LoginReturnDTO loginReturnDTO = MapUserToLoginReturn(user, userDB);
                        return loginReturnDTO;
                    }

                    throw new UserNotActiveException("Your account is not activated");
                }
                throw new UnauthorizedUserException("Invalid username or password");
            }catch
            (Exception ex)
            {
                throw ex;
            }
            
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
                var emailfound = await _userrepo.FindAll(ud => ud.Email ==  userDTO.Email);
                if (emailfound != null)
                {
                    throw new UserAlreadyExistException("User Account Already Exists, Please Login!");
                }
            }
            catch (Exception ex)
            {
                if (ex is not ObjectsNotFoundExceoption)
                    throw ex;
                try
                {
                    var emailfound = await _userrepo.FindAll(u => u.Email == userDTO.Email);
                    if (emailfound != null)
                    {
                        throw new UserAlreadyExistException("User Account Already Exists, Please Login!");
                    }
                }
                catch(Exception exp)
                {
                    if(exp is not ObjectsNotFoundExceoption)
                        throw exp;
                }
            }
            try
            {
                user = MapUserDTOToUser(userDTO);
                userdetails = MapUserDTOToUserDetail(userDTO);
                user = await _userrepo.Add(user);
                userdetails.UserId = user.Id;
                userdetails = await _userdetailrepo.Add(userdetails);
                RegisterReturnDTO registerReturnDTO = MapRegisterReturnDTO(user, userdetails);
                return registerReturnDTO;
            }
            catch (Exception) { }
            if (user != null)
                await RevertUserInsert(user);
            if (userdetails != null && user == null)
                await RevertUserDetailInsert(userdetails);
            throw new UnableToRegisterException("Not able to register at this moment");
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
            if (ud != null)
            {
                ud.Status = userUpdateStatusDTO.Status;
                await _userdetailrepo.Update(ud);
                return "User Status Successfully Updated";
            }
            throw new NoUserFoundException("No User Found");
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

        private async Task RevertUserDetailInsert(UserDetail userdetail)
        {
            await _userdetailrepo.Delete(userdetail);
        }

        private async Task RevertUserInsert(User user)
        {
            await _userrepo.Delete(user);
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
    }
}

