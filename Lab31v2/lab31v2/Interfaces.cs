namespace lab31v2
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
        void Add(User user);
        void Update(User user);
    }

    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}