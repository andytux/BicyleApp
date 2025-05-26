using BicyleApp.Data.Models;
using BicyleApp.Data;
using Microsoft.EntityFrameworkCore;
using BicyleApp.Data.ViewModels;

namespace BicyleApp.Services
{
    public class RankingService
    {
        private readonly AppDbContext db;

        public RankingService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<(User User, double TotalKm)>> GetTopByKmAsync(int competitionId, int topN = 10)
        {
            var result = await db.Rides
                .Where(r => r.CompetitionId == competitionId)
                .GroupBy(r => r.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalKm = g.Sum(r => r.DistanceKm)
                })
                .OrderByDescending(g => g.TotalKm)
                .Take(topN)
                .ToListAsync();

            return result.Select(r => (r.User, r.TotalKm)).ToList();
        }


        public async Task<List<(User User, int RideCount)>> GetTopByRideCountAsync(int competitionId, int topN = 10)
        {
            var result = await db.Rides
                .Where(r => r.CompetitionId == competitionId)
                .GroupBy(r => r.User)
                .Select(g => new
                {
                    User = g.Key,
                    RideCount = g.Count()
                })
                .OrderByDescending(g => g.RideCount)
                .Take(topN)
                .ToListAsync();

            return result.Select(r => (r.User, r.RideCount)).ToList();
        }

        public async Task<(List<WinnerViewModel>, List<WinnerViewModel>)> GetWinnersAsync(int competitionId)
        {
            var allRides = await db.Rides
                .Include(r => r.User)
                .Where(r => r.CompetitionId == competitionId)
                .ToListAsync();

            var kmWinners = allRides
                .GroupBy(r => r.UserId)
                .Select(g => new WinnerViewModel
                {
                    Name = g.First().User.Name,
                    City = g.First().User.City,
                    TotalKm = g.Sum(r => r.DistanceKm)
                })
                .OrderByDescending(x => x.TotalKm)
                .Take(10)
                .ToList();

            var kmWinnerIds = kmWinners.Select(x => x.Name).ToHashSet();

            var rideWinners = allRides
                .Where(r => !kmWinnerIds.Contains(r.User.Name))
                .GroupBy(r => r.UserId)
                .Select(g => new WinnerViewModel
                {
                    Name = g.First().User.Name,
                    City = g.First().User.City,
                    RideCount = g.Count()
                })
                .OrderByDescending(x => x.RideCount)
                .Take(10)
                .ToList();

            return (kmWinners, rideWinners);
        }


    }
}
