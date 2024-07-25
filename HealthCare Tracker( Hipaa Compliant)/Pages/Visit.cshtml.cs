using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace HealthCare.Pages
{
    public class ScheduleVisitModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ScheduleVisitModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string PatientID { get; set; }

        [BindProperty]
        public string DoctorID { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        public string VisitDate { get; set; }

        [BindProperty]
        public string Purpose { get; set; }

        public List<SelectListItem> Patients { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            // Populate patient and doctor dropdowns
            PopulatePatientDropdown();
            PopulateDoctorDropdown();

            return Page();
        }

        public IActionResult OnPost()
        {
            // Perform validation or additional logic if needed
            if (ModelState.IsValid)
            {
                // Call the stored procedure to schedule the visit
                ScheduleVisitInDatabase();

                // Display a success message
                TempData["SuccessMessage"] = "Visit scheduled successfully.";

                return RedirectToPage("/ViewVisit");
            }

            // If ModelState is not valid, repopulate dropdowns and redisplay the form
            PopulatePatientDropdown();
            PopulateDoctorDropdown();
            return Page();
        }

        private void PopulatePatientDropdown()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT PatientID, CONCAT(FirstName, ' ', LastName) AS FullName FROM Patient";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Patients.Add(new SelectListItem
                            {
                                Value = reader["PatientID"].ToString(),
                                Text = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }
        }

        private void PopulateDoctorDropdown()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DoctorID, Specialization FROM Doctor";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctors.Add(new SelectListItem
                            {
                                Value = reader["DoctorID"].ToString(),
                                Text = reader["Specialization"].ToString()
                            });
                        }
                    }
                }
            }
        }

        private void ScheduleVisitInDatabase()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("ScheduleVisit", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@PatientID", PatientID);
                        command.Parameters.AddWithValue("@DoctorID", DoctorID);
                        command.Parameters.AddWithValue("@VisitDate", DateTime.Parse(VisitDate));
                        command.Parameters.AddWithValue("@Purpose", Purpose);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, display an error message, etc.)
                ex.Message.ToString();
            }
        }
    }
}