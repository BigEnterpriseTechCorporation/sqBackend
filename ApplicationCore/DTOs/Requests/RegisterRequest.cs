namespace ApplicationCore.DTOs.Requests;

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    //adjust with more complex logic
    public string FullName { get; set; }
}