namespace Core.Services.Users;

public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserByName(string username);
    Task<User?> GetUserById(int userId);
    Task<bool> DoesUserExist(string username);
    Task AddUser(User user);
    Task<bool> RemoveUserById(int userId);
    Task<User?> UpdateUserAdmin(int userId, User updatedUser);
    Task<User?> UpdateUser(int userId, User updatedUser);
}
