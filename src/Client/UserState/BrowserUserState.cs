using Blazored.LocalStorage;

namespace BingoLingo.Client.UserState
{
    public class BrowserUserState : IUserState
    {
        private readonly ILocalStorageService _localStorage;

        public BrowserUserState(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task RememberUser()
        {
            var result = await Get();
            var newValue = result switch
            {
                null => new LocalUserData(Guid.NewGuid().ToString(), true),
                var v => v with { Remember = true },
            };
            await Set(newValue);
        }

        public async Task<bool> ShouldRememberUser()
        {
            var result = await Get();
            return result?.Remember ?? false;
        }

        public async Task<string> GetOrCreateUserId()
        {
            var result = await Get();
            var newValue = result switch
            {
                {
                    Remember: true
                } => result,
                _ => new LocalUserData(Guid.NewGuid().ToString(), false)
            };
            await Set(newValue);
            return newValue.Id;
        }

        private async Task<LocalUserData> Get()
        {
            return await _localStorage.GetItemAsync<LocalUserData>(nameof(LocalUserData));
        }

        private ValueTask Set(LocalUserData newValue)
        {
            return _localStorage.SetItemAsync(nameof(LocalUserData), newValue);
        }

        private record LocalUserData(string Id, bool Remember);
    }
}