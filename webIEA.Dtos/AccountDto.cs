using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public long loginUserId { get; set; }
        public string TableName { get; set; }
    }



  public  class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
  public  class UpdatePasswordDto
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }




}
