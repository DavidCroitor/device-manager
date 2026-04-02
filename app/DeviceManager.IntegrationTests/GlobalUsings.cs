global using System.Net;
global using System.Net.Http.Json;
global using Microsoft.Extensions.Configuration;
global using Microsoft.AspNetCore.Hosting;
global using FluentAssertions;
global using Microsoft.AspNetCore.Mvc.Testing;

global using Application.DTOs.DeviceDtos;
global using Application.DTOs.UserDtos;

public class ActionResponse { public string Message { get; set; } }