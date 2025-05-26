using BicyleApp.Data.Models;
using BicyleApp.Data;
using Microsoft.EntityFrameworkCore;
using static BicyleApp.Data.Enums.Enum;
using BicyleApp.Data.ViewModels;

namespace BicyleApp.Services
{
    public class CompetitionService
    {
        private readonly AppDbContext db;

        public CompetitionService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Competition>> GetAllAsync() =>
            await db.Competitions.AsNoTracking().ToListAsync();

        public async Task<Competition?> GetByIdAsync(int id) =>
            await db.Competitions.FindAsync(id);

        public async Task AddAsync(Competition comp)
        {
            db.Competitions.Add(comp);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CompetitionViewModel comp)
        {
            var compToUpdate = await GetByIdAsync(comp.CompetitionId);
            if (compToUpdate != null)
            {
                compToUpdate.StartDate = comp.StartDate;
                compToUpdate.EndDate = comp.EndDate;
                compToUpdate.Status = comp.Status;
                compToUpdate.Title = comp.Title;

                db.Competitions.Update(compToUpdate);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await db.Competitions.FindAsync(id);
            if (entity != null)
            {
                db.Competitions.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Competition?> GetActiveCompetitionAsync()
        {
            var today = DateTime.UtcNow;

            return await db.Competitions.FirstOrDefaultAsync(c =>
                c.Status == CompetitionStatus.Active &&
                c.StartDate <= today &&
                c.EndDate >= today);
        }


        public async Task<List<Competition>> GetActiveAsync()
        {
            return await db.Competitions
                .Where(c => c.Status == CompetitionStatus.Active)
                .ToListAsync();
        }

    }
}
