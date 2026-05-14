using System;
using System.Collections.Generic;
using System.Text;

using Zorent.Domain.Entities;

namespace Zorent.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string email, string username);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> UsernameExistsAsync(string username);

        Task AddUserAsync(User user);

        Task SaveChangesAsync();

        Task<User?> GetByUsernameAsync(string username);

        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}