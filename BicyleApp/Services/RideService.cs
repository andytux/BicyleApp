using BicyleApp.Data.Models;
using BicyleApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BicyleApp.Services
{
    public class RideService
    {
        private readonly AppDbContext db;

        public RideService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Ride>> GetAllAsync() =>
            await db.Rides.Include(r => r.User).Include(r => r.Competition).ToListAsync();

        public async Task<List<Ride>> GetByUserAsync(int userId) =>
            await db.Rides
                .Where(r => r.UserId == userId)
                .Include(r => r.Competition)
                .ToListAsync();

        public async Task<List<Ride>> GetByCompetitionAsync(int competitionId) =>
            await db.Rides
                .Where(r => r.CompetitionId == competitionId)
                .Include(r => r.User)
                .ToListAsync();

        public async Task<Ride?> GetByIdAsync(int id) =>
            await db.Rides.FindAsync(id);

        public async Task AddAsync(Ride ride)
        {
            db.Rides.Add(ride);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ride ride)
        {
            db.Rides.Update(ride);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await db.Rides.FindAsync(id);
            if (entity != null)
            {
                db.Rides.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> RideExistsOnDateAsync(int userId, DateTime date)
        {
            return await db.Rides.AnyAsync(r => r.UserId == userId && r.Date.Date == date.Date);
        }
    }
}
