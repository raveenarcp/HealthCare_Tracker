using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace YourNamespace.Pages
{
    public class VisitModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public VisitModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<VisitData> Visits { get; set; }

        public void OnGet()
        {
            LoadVisitData();
        }

        private void LoadVisitData()
        {
            Visits = new List<VisitData>();
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT v.VisitID, v.VisitDate, v.Purpose, v.Status, v.DoctorID, v.PatientID, d.DoctorName, p.FirstName + ' ' + p.LastName AS PatientName " +
                                                            "FROM Visit v " +
                                                            "INNER JOIN Doctor d ON d.DoctorID = v.DoctorID " +
                                                            "INNER JOIN Patient p ON p.PatientID = v.PatientID", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            VisitData visit = new VisitData
                            {
                                VisitID = reader["VisitID"].ToString(),
                                VisitDate = reader["VisitDate"].ToString(),
                                Purpose = reader["Purpose"].ToString(),
                                Status = reader["Status"].ToString(),
                                DoctorID = reader["DoctorID"].ToString(),
                                PatientID = reader["PatientID"].ToString(),
                                DoctorName = reader["DoctorName"].ToString(),
                                PatientName = reader["PatientName"].ToString()
                            };

                            Visits.Add(visit);
                        }
                    }
                }
            }
        }
    }

    public class VisitData
    {
        public string VisitID { get; set; }
        public string VisitDate { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; }
        public string DoctorID { get; set; }
        public string PatientID { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
    }
}
