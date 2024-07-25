using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HealthCare.Pages.Admission
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AdmissionRecord> Admissions { get; set; }

        public IActionResult OnGet()
        {
            // Fetch the admission records for display
            LoadAdmissions();
            return Page();
        }

        public IActionResult OnPostInsertAdmission()
        {
            try
            {
                InsertAdmissionFromTreatment();
                TempData["SuccessMessage"] = "Admission successfully inserted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            // Refresh the list of admissions after insertion
            LoadAdmissions();
            return Page();
        }

        private void InsertAdmissionFromTreatment()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("InsertAdmissionFromTreatment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting admission: " + ex.Message);
            }
        }

        private void LoadAdmissions()
        {
            Admissions = new List<AdmissionRecord>();

            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT AdmissionID, TreatmentID, RoomID, AdmissionDate, isnull(DischargeDate,''), AdmissionReason, Status FROM Admission", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AdmissionRecord admission = new AdmissionRecord
                            {
                                AdmissionID = reader.GetInt32(0),
                                TreatmentID = reader.GetString(1),
                                RoomID = reader.GetString(2),
                                AdmissionDate = reader.GetDateTime(3),
                                DischargeDate = reader.GetDateTime(4),
                                AdmissionReason = reader.GetString(5),
                                Status = reader.GetString(6)
                            };

                            Admissions.Add(admission);
                        }
                    }
                }
            }
        }
    }

    public class AdmissionRecord
    {
        public int AdmissionID { get; set; }
        public string TreatmentID { get; set; }
        public string RoomID { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime DischargeDate { get; set; }
        public string AdmissionReason { get; set; }
        public string Status { get; set; }
    }
}
