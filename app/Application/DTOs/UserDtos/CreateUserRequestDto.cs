using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.UserDtos;

public class CreateUserRequestDto
{
    public string Name { get; set;}
    public string Email { get; set; }
    public string Role { get; set; }
    public string Location { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
