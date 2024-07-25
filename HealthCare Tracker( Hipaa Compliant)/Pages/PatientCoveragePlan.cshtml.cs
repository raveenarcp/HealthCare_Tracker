// PatientCoveragePlanModel.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HealthCare.Pages
{
    public class PatientCoveragePlanModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public PatientCoveragePlanModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string SelectedPatientId { get; set; }

        [BindProperty]
        public string SelectedPlanId { get; set; }

        public List<Patients> Patientslist { get; set; }
        public List<CoveragePlan> CoveragePlans { get; set; }

        public IActionResult OnGet()
        {
            // Ensure that Patientslist and CoveragePlans are always initialized
            Patientslist = new List<Patients>();
            CoveragePlans = new List<CoveragePlan>();

            PopulatePatients();
            PopulateCoveragePlans();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Insert the selected patient and coverage plan into PatientCoveragePlan table
                InsertPatientCoveragePlan();

                TempData["SuccessMessage"] = "Plan successfully selected for the patient.";
            }

            // Repopulate the dropdowns
          

            return Page();
        }

        private void InsertPatientCoveragePlan()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //using (SqlCommand command = new SqlCommand("Insert into PatientCoveragePlan(PlanID,PatientID) values (@PatientID,@PlanID) ", connection))
                //{

                //    command.Parameters.AddWithValue("@PatientID", SelectedPatientId);
                //    command.Parameters.AddWithValue("@PlanID", SelectedPlanId);

                //    command.ExecuteNonQuery();
                //}
                using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM PatientCoveragePlan WHERE PatientID = @PatientID AND PlanID = @PlanID", connection))
                {
                    checkCommand.Parameters.AddWithValue("@PatientID", SelectedPatientId);
                    checkCommand.Parameters.AddWithValue("@PlanID", SelectedPlanId);

                    int existingRecordCount = (int)checkCommand.ExecuteScalar();

                    if (existingRecordCount > 0)
                    {
                        Console.WriteLine("Record already exists.");
                    }
                    else
                    {
                        // Record doesn't exist, proceed with the insertion
                        using (SqlCommand command = new SqlCommand("INSERT INTO PatientCoveragePlan (PlanID, PatientID) VALUES (@PlanID, @PatientID)", connection))
                        {
                            command.Parameters.AddWithValue("@PatientID", SelectedPatientId);
                            command.Parameters.AddWithValue("@PlanID", SelectedPlanId);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void PopulatePatients()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PatientID, isnull(FirstName,'')FirstName FROM Patient", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Patients patient = new Patients
                            {
                                PatientID = reader["PatientID"].ToString(),
                                FirstName = reader["FirstName"].ToString()
                            };

                            Patientslist.Add(patient);
                        }
                    }
                }

            }
        }

        private void PopulateCoveragePlans()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PlanID, PlanName FROM CoveragePlan", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CoveragePlan plan = new CoveragePlan
                            {
                                PlanID = reader["PlanID"].ToString(),
                                PlanName = reader["PlanName"].ToString()
                            };

                            CoveragePlans.Add(plan);
                        }
                    }
                }
            }
        }
    }

    public class Patients
    {
        public string PatientID { get; set; }
        public string FirstName { get; set; }
    }

    public class CoveragePlan
    {
        public string PlanID { get; set; }
        public string PlanName { get; set; }
    }
}
