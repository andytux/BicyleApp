using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static BicyleApp.Data.Enums.Enum;

namespace BicyleApp.Services
{
    public class AuthStateService
    {
        private readonly ProtectedSessionStorage session;

        public bool IsLoggedIn { get; private set; }
        public string? Email { get; private set; }
        public int? UserId { get; private set; }
        public UserRole Role { get; private set; }

        public event Action? OnChange;

        public AuthStateService(ProtectedSessionStorage session)
        {
            this.session = session;
        }

        public async Task LoginAsync(string email, int userId, UserRole role)
        {
            IsLoggedIn = true;
            Email = email;
            UserId = userId;
            Role = role;

            await session.SetAsync(nameof(Email), email);
            await session.SetAsync(nameof(UserId), userId);
            await session.SetAsync(nameof(Role), role);

            Notify();
        }

        public async Task LogoutAsync()
        {
            IsLoggedIn = false;
            Email = null;
            UserId = null;
            Role = UserRole.None;

            await session.DeleteAsync(nameof(Email));
            await session.DeleteAsync(nameof(UserId));
            await session.DeleteAsync(nameof(Role));

            Notify();
        }

        public async Task InitializeAsync()
        {
            var email = await session.GetAsync<string>(nameof(Email));
            var id = await session.GetAsync<int>(nameof(UserId));
            var role = await session.GetAsync<UserRole>(nameof(Role));

            if (email.Success && id.Success && role.Success)
            {
                IsLoggedIn = true;
                Email = email.Value;
                UserId = id.Value;
                Role = role.Value;
                Notify();
            }
        }

        private void Notify()
        {
            OnChange?.Invoke();
        } 
    }
}
