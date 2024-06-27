namespace Reddit.Models
{
    public class AuthRes
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set;}
        public DateTime RefreshTokenExpiration { get; set;}
    }
}
