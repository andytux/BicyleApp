using BicyleApp.Data;
using BicyleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BicyleApp.Services
{
    public class UserService
    {
        private readonly AppDbContext dbContext;

        public UserService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await dbContext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await dbContext.Users.FindAsync(userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
