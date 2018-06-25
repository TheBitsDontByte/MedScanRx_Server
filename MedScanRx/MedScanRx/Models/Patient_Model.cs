using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Models
{
    public class Patient_Model
    {
        public long PatientId { get; set; }

        [Required(ErrorMessage = "A date of birth is required")]
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthDateOnly => DateOfBirth.Date.ToString("yyyy-MM-dd");

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "A gender is required")]
        [RegularExpression(@"[FMfm]", ErrorMessage = "A gender must be either 'F' or 'M'")]
        [StringLength(1, ErrorMessage = "Gender must be one character only")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "A first name is required")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "A last name is required")]
        [StringLength(50, ErrorMessage = "Last name must be less than 50 characters")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";


        [Phone(ErrorMessage = "Primary phone number is invalid")]
        [Required(ErrorMessage = "A primary phone number is required")]
        [StringLength(12, ErrorMessage = "Primary phone number must be 7 or 10 numeric characters")]
        public string Phone1 { get; set; }

        [Phone(ErrorMessage = "Secondary phone number is invalid")]
        [StringLength(12, ErrorMessage = "Secondary phone number must be 7 or 10 numeric characters")]
        public string Phone2 { get; set; }

        [EmailAddress(ErrorMessage = "Email address is invalid")]
        [StringLength(100, ErrorMessage = "Email address must be less than 100 characters")]
        [Required(ErrorMessage = "An email address is required")]
        public string Email { get; set; }
        [StringLength(100, ErrorMessage = "Emergency contact name must be less than 100 characters")]
        public string EmergencyContactName { get; set; }

        [StringLength(25, ErrorMessage = "Emergency contact relationship must be less than 25 characters")]
        public string EmergencyContactRelation { get; set; }

        [Phone(ErrorMessage = "Emergency contact phone number is invalid")]
        [StringLength(12, ErrorMessage = "Emergency contact phone number must be 7 or 10 numeric characters")]
        public string EmergencyContactPhone { get; set; }

        [StringLength(50, ErrorMessage = "Preferred hospital must be less than 50 characters")]
        public string PreferredHospital { get; set; }

        [StringLength(100, ErrorMessage = "Preferred physician must be less than 100 characters")]
        public string PreferredPhysician { get; set; }
    }
}
