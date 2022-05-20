using AutoMapper;
using BLL.Models;
using BLL.Services.CustomExceptions;
using BLL.Services.Interfaces;
using BLL.Services.PasswordHash;
using DAL.Entities;
using DAL.Uow;

namespace BLL.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _database;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash;

        public UsersService(
            IUnitOfWork database,
            IMapper mapper,
            IPasswordHash passwordHash)
        {
            _database = database;
            _mapper = mapper;
            _passwordHash = passwordHash;
        }

        public UserDto CreateUser(UserDto userDto)  
        {
            var user = _mapper.Map<UserDto, User>(userDto);

            user.Id = Guid.NewGuid();
            user.Password = _passwordHash.EncryptPassword(user.Password, user.Id.ToByteArray());
            user.Role = RoleNames.User;

            _database.Users.Insert(user);
            _database.Save();

            return _mapper.Map<User, UserDto>(user);
        }

        public UserDto EditUser(UserDto userDto, Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var users = _database.Users.Get().ToList();
            return _mapper.Map<List<User>, List<UserDto>>(users); 
        }

        public UserDto ValidateUser(UserDto userDto)
        {
            if(userDto is null)
            {
                throw new UserValidationException("Access denied. Unresolved user from request body.");
            }

            User user = _database.Users.Get(user =>
                user.EMail == userDto.EMail)
                ?.First() 
                ?? throw new UserValidationException("Access denied. Unresolved email."); 
            
            var incomingPasswordHash = _passwordHash.EncryptPassword(userDto.Password, user.Id.ToByteArray());

            if (incomingPasswordHash != user.Password)
            {
                throw new UserValidationException("Access denied. Incorrect password.");
            }

            return _mapper.Map<User, UserDto>(user);
        }
    }
}
