using Api.Controllers.Auth;

namespace Api.Controllers.Users;

[Route("api/user")]
[ApiController]
public class UserController(IUserService userService, Validator validator) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly Validator _validator = validator;


    [HttpGet("get")]
    public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsers();

            if(users == null) return NotFound("No users where found");

            return Ok(GetUserDTO(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/name/{name}")]
    public async Task<ActionResult<UserDTO>> GetUserByName(string name)
    {
        try
        {
            var user = await _userService.GetUserByName(name);
            if (user == null)
                return NotFound($"User with name: {name} was not found");

            var newUser = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Tickets = GetTicketDTO(user.Tickets)
            };

            return Ok(newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("get/id/{userId}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int userId)
    {
        try
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
                return NotFound($"User with name: {userId} was not found");

            var newUser = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Tickets = GetTicketDTO(user.Tickets)
            };

            return Ok(newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<User>> DeleteUserById(int id)
    {
        try
        {
            if (!await _userService.RemoveUserById(id))
                return NotFound("User not found.");

            return Ok(id);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("update/admin/{id}")]
    public async Task<ActionResult<User>> UpdateUserAdmin(int id, [FromBody] UserCreateDTO user) //update function used by admin
    {
        var validation = _validator.Validate(new UserValidator(), user);
        if (validation != null)
            return validation;

        var newUser = new User
        {
            Id = id,
            Username = user.Username,
            PasswordHash = "temppass",
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = DateTime.Now,
            Tickets = null!
        };

        try
        {
            var updatedUser = await _userService.UpdateUserAdmin(id, newUser);
            if (updatedUser == null)
            {
                return NotFound($"User with id: {id} not found.");
            }
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UserUpdateDTO user) //updatefunction used by users
    {
        
        var validation = _validator.Validate(new UserUpdateValidator(), user);
        if (validation != null)
            return validation;

        var newUser = new User
        {
            Id = id,
            Username = user.Username,
            PasswordHash = user.Password,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = UserRole.User,
            CreatedAt = DateTime.Now,
            Tickets = null!
        };

        try
        {
            var updatedUser = await _userService.UpdateUser(id, newUser);
            if (updatedUser == null)
            {
                return NotFound($"User with id: {id} not found.");
            }
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] UserCreateDTO user)
    {

        var validation = _validator.Validate(new UserValidator(), user);
        if (validation != null)
            return validation;

        var newUser = new User
        {
            Username = user.Username,
            PasswordHash = "password",
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            CreatedAt = DateTime.Now,
            Tickets = null!
        };
        bool userExists = await _userService.DoesUserExist(user.Username);
        if (userExists == true)
        {
            return BadRequest("Username already exists");
        }

        await _userService.AddUser(newUser);
        return Created("user created successfully", new Response(newUser.Username));
    }

    private static List<UserDTO> GetUserDTO(List<User> users)
    {
        return users.Select(t => new UserDTO
        {
            Id = t.Id,
            Username = t.Username,
            Email = t.Email,
            PhoneNumber = t.PhoneNumber,
            Role = t.Role,
            CreatedAt = t.CreatedAt,
            Tickets = GetTicketDTO(t.Tickets)
        }).ToList();
    }

    private static List<TicketDTO> GetTicketDTO(List<Ticket> tickets)
    {
        return tickets.Select(t => new TicketDTO
        {
            Id = t.Id,
            UserId = t.UserId,
            EventId = t.EventId,
            Title = t.Title,
            Seat = t.Seat,
            Price = t.Price
        }).ToList();
    }
}
