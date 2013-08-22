using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ContactManager.Models
{
    public class Contact
    {
        [ScaffoldColumn(false)]
        public int ContactId { get; set; }
        [Required]
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
    }
}