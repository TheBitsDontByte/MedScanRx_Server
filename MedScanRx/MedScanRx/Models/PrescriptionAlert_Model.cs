using System;

namespace MedScanRx.Models
{
    public class PrescriptionAlert_Model
    {
        public DateTime AlertDateTime { get; set; }
        public DateTime? TakenDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
