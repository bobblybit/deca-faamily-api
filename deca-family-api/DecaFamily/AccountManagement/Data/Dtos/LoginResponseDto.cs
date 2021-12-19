namespace AccountManagement.Data.Dtos
{
    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string Fullname { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string PhotoUrl { get; set; }
    }
}
