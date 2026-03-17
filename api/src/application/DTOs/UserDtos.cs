namespace api.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreateUserDto
{
    public required string Name { get; set; }
    public int Age { get; set; }
}

public class UpdateUserDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Age { get; set; }
}
