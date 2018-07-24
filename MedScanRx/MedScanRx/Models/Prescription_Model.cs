using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Models
{
    public class Prescription_Model
    {
        public int PrescriptionId { get; set; }
        public long PatientId { get; set; }
        public string PrescriptionName { get; set; }
        public string ImageUrl { get; set; }
        public string Rxcui { get; set; }
        public string Ndc { get; set; }
        public string NextAlert { get; set; }
        public string Color { get; set; }
        public string Dosage { get; set; }
        public string Identifiers { get; set; }
        public string Shape { get; set; }
        public string DoctorNotes { get; set; }
        public string Warnings { get; set; }
        public string OriginalNumberOfDoses { get; set; }
        public string CurrentNumberOfDoses { get; set; }
        public string OriginalNumberOfRefills { get; set; }
        public string CurrentNumberOfRefills { get; set; }
        public string IsActive { get; set; }
        public List<PrescriptionAlert_Model> ScheduledAlerts { get; set; }
    }
}
