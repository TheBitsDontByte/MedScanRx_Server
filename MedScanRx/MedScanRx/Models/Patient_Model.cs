using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Models
{
    public class Patient_Model
    {
        public long PatientId { get; set; }

        public DateTime DateOfBirth { get; set; }
       
        //This may not be good as a char ?
        public string Gender { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string PreferredHospital { get; set; }
        public string PreferredPhysician { get; set; }
    }
}
