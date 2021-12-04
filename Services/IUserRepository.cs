﻿using System.Threading.Tasks;
using YoghurtBank.Data.Model;
using System.Net;

namespace YoghurtBank.Services
{
    public interface IUserRepository
    {
        Task<(HttpStatusCode code, UserDetailsDTO details)> FindUserByIdAsync(int userId);

        Task<UserDetailsDTO> CreateAsync(UserCreateDTO user);

        Task<int> DeleteAsync(int id);
       
    }
}
