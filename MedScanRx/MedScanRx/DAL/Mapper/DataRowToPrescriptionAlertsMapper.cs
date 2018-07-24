using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL.Mapper
{
    public static class DataRowToPrescriptionAlertsMapper
    {
        public static PrescriptionAlert_Model Map(SqlDataReader reader)
        {
            try
            {
                bool couldParse = DateTime.TryParse(reader["TakenDateTime"].ToString(), out DateTime takenDateTime);

                var prescriptionAlert = new PrescriptionAlert_Model
                {
                    AlertDateTime = DateTime.Parse(reader["AlertDateTime"].ToString()),
                    IsActive = bool.Parse(reader["IsActive"].ToString())
                };

                if (couldParse)
                    prescriptionAlert.TakenDateTime = takenDateTime;
                

                return prescriptionAlert;

            }
            catch (Exception ex)
            {
                throw new MappingException("Mapping a data row to a prescription alert failed", ex);
            }
        }

    }
}
