﻿using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IUsersService
    {
        public UserDto CreateUser(UserDto userDto);
        public IEnumerable<UserDto> GetUsers(); 
        public UserDto ValidateUser(UserDto userDto);
        public UserDto EditUser(UserDto userDto, string id);
    }
}
