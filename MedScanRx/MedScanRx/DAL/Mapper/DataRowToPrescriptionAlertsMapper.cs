using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.DAL.Mapper
{
    public static class DataRowToPrescriptionAlertsMapper
    {
        public static string Map(SqlDataReader reader)
        {
            return reader["AlertDateTime"].ToString();
        }

    }
}
