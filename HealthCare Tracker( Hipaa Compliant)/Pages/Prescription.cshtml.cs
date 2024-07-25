using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace HealthCare.Pages.Prescription
{
    public class CreatePrescriptionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreatePrescriptionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string TreatmentID { get; set; }

        [BindProperty]
        public string MedicineName { get; set; }

        [BindProperty]
        public DateTime PrescriptionDate { get; set; }

        [BindProperty]
        public string Dosage { get; set; }

        public List<Treatment> Treatments { get; set; }

        public void OnGet()
        {
            // Load treatments where there is no corresponding entry in the Prescription table
            LoadAvailableTreatments();
        }

        public IActionResult OnPost()
        {
            System.Diagnostics.Debug.WriteLine(TreatmentID);


            if (ModelState.IsValid)
            {
                // Insert into Prescription table
                InsertIntoPrescription();

                TempData["SuccessMessage"] = "Prescription created successfully.";
                return RedirectToPage("/Prescription");
            }

            // If the model state is not valid, reload available treatments
            LoadAvailableTreatments();

            return Page();
        }

        private void LoadAvailableTreatments()
        {
            Treatments = new List<Treatment>();
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT t.TreatmentID FROM Treatment t " +
                    "LEFT JOIN Prescription p ON p.TreatmentID = t.TreatmentID WHERE p.TreatmentID IS NULL", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Treatments.Add(new Treatment
                            {
                                TreatmentID = reader.GetString(0)
                            }); 
                        }
                    }
                }
            }
        }

        private void InsertIntoPrescription()
        {
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string maxPrescriptionIDQuery = "SELECT MAX(PrescriptionID) FROM Prescription";
                string maxPrescriptionIDResult = "";

                using (SqlCommand maxIdCommand = new SqlCommand(maxPrescriptionIDQuery, connection))
                {
                    maxPrescriptionIDResult = maxIdCommand.ExecuteScalar().ToString();
                }

                // Use the SQL Server function to generate the new DoctorID
                string newPrescriptionIDQuery = $"SELECT dbo.GenerateNewPrimaryKey('{maxPrescriptionIDResult}', 'PRE0')";
                string newPrescriptionIDResult = "";
                using (SqlCommand newIdCommand = new SqlCommand(newPrescriptionIDQuery, connection))
                {
                    newPrescriptionIDResult = newIdCommand.ExecuteScalar().ToString();
                    // Now newDoctorIdResult contains the updated DoctorID
                }

                using (SqlCommand command = new SqlCommand("INSERT INTO Prescription (PrescriptionID,TreatmentID, MedicineName, PrescriptionDate, Dosage) " +
                    "VALUES (@PrescriptionID,@TreatmentID, @MedicineName, @PrescriptionDate, @Dosage)", connection))
                {
                    command.Parameters.AddWithValue("@PrescriptionID", newPrescriptionIDResult);
                    command.Parameters.AddWithValue("@TreatmentID", TreatmentID);
                    command.Parameters.AddWithValue("@MedicineName", MedicineName);
                    command.Parameters.AddWithValue("@PrescriptionDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Dosage", Dosage);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
  
    public class Treatment
    {
        public string TreatmentID { get; set; }
    }
}
