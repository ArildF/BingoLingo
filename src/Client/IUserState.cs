namespace BingoLingo.Client;

public interface IUserState
{
    Task RememberUser();
    Task<string> GetOrCreateUserId();
    Task<bool> ShouldRememberUser();
}