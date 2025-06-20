﻿namespace ContactManager.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? Keywords { get; set; } 
    }
}
