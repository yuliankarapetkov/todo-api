﻿using System.ComponentModel.DataAnnotations;

namespace TodoApi.Web.Models
{
    public class CredentialModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
