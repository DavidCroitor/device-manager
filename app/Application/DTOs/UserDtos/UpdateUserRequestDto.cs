using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.UserDtos;

public class UpdateUserRequestDto
{
    public string Name { get; set;}
    public string Role { get; set; }
    public string Location { get; set; }
}
