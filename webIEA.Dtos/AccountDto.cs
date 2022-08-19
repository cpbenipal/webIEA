using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
   public enum IEARoles
    {
        Admin=1,
        Member=2,
        Provider=3,
        Intern=4
    }
    public class AccountDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public int RoleId { get; set; }
        public long loginUserId { get; set; }
        public string TableName { get; set; }
    }



  public  class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
  public  class UpdatePasswordDto
    {
        public string Id { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}", ErrorMessage = "Password must be atleast 8 characters with one at least one lower case, one upper case, one number and one special character ")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }

    }




}
