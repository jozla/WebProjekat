using Common.Enums;

namespace Common.DTOs
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly Birthday { get; set; }
        public string Address { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string Image { get; set; } = string.Empty;
        public VerificationState VerificationState { get; set; } = VerificationState.Verified;
    }
}
