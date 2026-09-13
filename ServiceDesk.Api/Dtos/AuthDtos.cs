using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Dtos;

public record RegisterDto(string FullName, string Email, string Password, UserRole Role);

public record LoginDto(string Email, string Password);