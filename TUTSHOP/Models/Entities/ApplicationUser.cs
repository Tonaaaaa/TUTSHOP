using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
       
        [Required]
        [Display(Name = "Tên")]
        public string? FullName {  get; set; }
        public bool Gender {  get; set; }
        public DateTime? Birthday { get; set; }
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        [Display(Name = "Số điện thoại")]
        public bool IsBlocked { get; set; }
    }
}
