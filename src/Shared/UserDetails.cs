namespace BingoLingo.Shared
{
    public class UserDetails
    {
        public string? IdentityName { get; }

        public UserDetails(string? identityName)
        {
            IdentityName = identityName;
        }

        public UserPreferences UserPreferences { get; set; } = new();
    }
}