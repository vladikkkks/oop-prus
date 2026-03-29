using System;

namespace lab31v2
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        // Впровадження залежностей (DI) через конструктор
        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public bool Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var existingUser = _userRepository.GetByUsername(username);
            if (existingUser != null)
                return false; // Користувач вже існує

            var hash = _passwordHasher.HashPassword(password);
            
            _userRepository.Add(new User 
            { 
                Username = username, 
                PasswordHash = hash 
            });

            return true;
        }

        public bool Login(string username, string password)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
                return false;

            return _passwordHasher.VerifyPassword(password, user.PasswordHash);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _userRepository.GetByUsername(username);
            if (user == null)
                return false;

            if (!_passwordHasher.VerifyPassword(oldPassword, user.PasswordHash))
                return false; // Старий пароль не співпадає

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _userRepository.Update(user);

            return true;
        }
    }
}