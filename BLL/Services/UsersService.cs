using AutoMapper;
using BLL.Models;
using BLL.Services.CustomExceptions;
using BLL.Services.Interfaces;
using BLL.Services.PasswordHash;
using DAL.Entities;
using DAL.Extensions;
using DAL.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Text;

namespace BLL.Services
{
    public class UsersService : IUsersService
    {
        private readonly IMongoRepository<User> _database;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash;

        public UsersService(
            IMongoRepository<User> database,
            IMapper mapper,
            IOptions<MongoDbConfig> dbOptions,
            IPasswordHash passwordHash)
        {
            _database = database;
            _database.UseCollection(dbOptions.Value.UsersCollectionName);
            //_database.SetFieldAsUnique("EMail");
            _mapper = mapper;
            _passwordHash = passwordHash;
        }

        public UserDto CreateUser(UserDto userDto)  
        {
            var user = _mapper.Map<UserDto, User>(userDto);

            user.Id = ObjectId.GenerateNewId().ToString();
            user.Password = _passwordHash.EncryptPassword(user.Password, Encoding.ASCII.GetBytes(user.Id));
            user.Role = "User";

            _database.InsertOne(user);

            return _mapper.Map<User, UserDto>(user);
        }

        public UserDto EditUser(UserDto userDto, string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var users = _database.FilterBy().ToList();
            return _mapper.Map<List<User>, List<UserDto>>(users); 
        }

        public UserDto ValidateUser(UserDto userDto)
        {
            if(userDto is null)
            {
                throw new UserValidationException("Access denied. Unresolved user from request body.");
            }

            User user = _database.FilterBy(user =>
                user.EMail == userDto.EMail)
                .FirstOrDefault() 
                ?? throw new UserValidationException("Access denied. Unresolved email."); 
            
            var incomingPasswordHash = _passwordHash.EncryptPassword(userDto.Password, Encoding.ASCII.GetBytes(user.Id));

            if (incomingPasswordHash != user.Password)
            {
                throw new UserValidationException("Access denied. Incorrect password.");
            }

            return _mapper.Map<User, UserDto>(user);
        }
    }
}
