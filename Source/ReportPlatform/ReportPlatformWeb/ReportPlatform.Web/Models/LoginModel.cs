using System.ComponentModel.DataAnnotations;

namespace ReportPlatform.Web.Models
{
    /// <summary>
    /// 登入模型
    /// </summary>
    public record LoginModel
    {
        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Required]
        public required string Username { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        [Required]
        public required string Password { get; set; }
    }
}
