using DizzlrApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DizzlrApp.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [PersonalData]
        [MaxLength(100)]
        public string LastName { get; set; }
        public List<File> UserFiles { get; set; }
    }
}
