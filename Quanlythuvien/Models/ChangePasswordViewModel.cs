

namespace Quanlythuvien.Models
{
    public class ChangePasswordViewModel
    {
        public int LibraId { get; set; }
        public string Username { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
