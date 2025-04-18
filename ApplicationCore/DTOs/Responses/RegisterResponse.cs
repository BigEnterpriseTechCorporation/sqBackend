namespace ApplicationCore.DTOs.Responses;

public class RegisterResponse
{
    public Guid Id  { get; set; }
    public string Token { get; set; }
    public string Role  { get; set; }
}