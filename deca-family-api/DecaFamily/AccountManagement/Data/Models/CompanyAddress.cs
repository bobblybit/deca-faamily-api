﻿using System.ComponentModel.DataAnnotations;

namespace AccountManagement.Data.Models
{
    public class CompanyAddress : BaseEntity
    {
        public string CompanyId { get; set; }
        public Company Company { get; set; }

        [MaxLength(125, ErrorMessage = "Street cannot be more than 125")]
        public string Street { get; set; }

        [MaxLength(125, ErrorMessage = "City cannot be more than 125")]
        public string City { get; set; }

        [MaxLength(125, ErrorMessage = "State cannot be more than 125")]
        public string State { get; set; }

        [MaxLength(125, ErrorMessage = "Country cannot be more than 125")]
        public string Country { get; set; }
    }
}