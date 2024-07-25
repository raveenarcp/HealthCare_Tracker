using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HealthCare.Pages
{
    public class TreatmentListModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public TreatmentListModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Treatments> Treatmentslist { get; set; }

        public void OnGet()
        {
            // Fetch and populate the list of treatments
            // Replace this with your actual data retrieval logic
            Treatmentslist = FetchTreatmentsFromDatabase();
        }

        private List<Treatments> FetchTreatmentsFromDatabase()
        {
            // Implement logic to fetch treatments from the database
            // This is just a placeholder; replace it with your actual logic
            string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Treatment", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Treatments> treatments = new List<Treatments>();

                        while (reader.Read())
                        {
                            Treatments treatment = new Treatments
                            {
                                TreatmentID = reader["TreatmentID"].ToString(),
                                TreatmentName = reader["TreatmentName"].ToString(),
                                TreatmentFees = Convert.ToDecimal(reader["TreatmentFees"]),
                                VisitID = reader["VisitID"].ToString(),
                                Admit_Flag = reader["Admit_Flag"].ToString(),
                                Notes = reader["Notes"].ToString()
                            };

                            treatments.Add(treatment);
                        }

                        return treatments;
                    }
                }
            }
        }
    }

    public class Treatments
    {
        public string TreatmentID { get; set; }
        public string TreatmentName { get; set; }
        public decimal TreatmentFees { get; set; }
        public string VisitID { get; set; }
        public string Admit_Flag { get; set; }
        public string Notes { get; set; }
    }
}
