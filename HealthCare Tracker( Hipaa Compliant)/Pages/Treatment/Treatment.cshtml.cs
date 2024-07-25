using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace HealthCare.Pages
{
    public class InsertTreatmentModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public InsertTreatmentModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string VisitID { get; set; }

        [BindProperty]
        public decimal TreatmentFees { get; set; }

        [BindProperty]
        public string Admit_Flag { get; set; }

        public List<string> VisitIDs { get; set; }

        public IActionResult OnGet()
        {
            // Fetch and populate VisitIDs in your OnGet method
            PopulateVisitIDs();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Call the stored procedure InsertTreatment
                    InsertTreatmentInDatabase();

                    // Optionally, you can add a success message to TempData
                    TempData["SuccessMessage"] = "Treatment successfully inserted.";
                }
                catch (Exception ex)
                {
                    // Handle the exception if needed
                    TempData["ErrorMessage"] = "Error inserting treatment.";
                }

                // You can redirect to another page or stay on the same page
                // Example: Redirect to the Index page for treatments
                return RedirectToPage("/Treatment/ViewTreatment");
            }

            // If ModelState is not valid, redisplay the form
            PopulateVisitIDs(); // Populate VisitIDs again in case of redisplay
            return Page();
        }

        private void InsertTreatmentInDatabase()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("HealthCareDatabase")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("InsertTreatment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@VisitID", VisitID);
                    command.Parameters.AddWithValue("@Admit_Flag", Admit_Flag);
                    command.Parameters.AddWithValue("@TreatmentFees", TreatmentFees);
              
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PopulateVisitIDs()
        {
            // Populate VisitIDs from your database
            // Example:
            VisitIDs = FetchVisitIDsFromDatabase();
        }

        private List<string> FetchVisitIDsFromDatabase()
        {
            // Implement logic to fetch VisitIDs from your database
            // Example: You might use a SELECT statement to get distinct VisitIDs from the Visit table

            List<string> visitIDs = new List<string>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("HealthCareDatabase")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT v.VisitID FROM Visit v LEFT JOIN Treatment t ON t.VisitID = v.VisitID WHERE t.VisitID IS NULL", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            visitIDs.Add(reader["VisitID"].ToString());
                        }
                    }
                }
            }

            return visitIDs;
        }
    }
}
