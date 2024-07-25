using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace HealthCare.Pages.Doctor
{
    public class UpdateDoctorModel : PageModel
    {
        public DoctorData UpdateDoctor = new DoctorData();

        public IActionResult OnGet()
        {
            if (!Request.Query.ContainsKey("id"))
            {
                // Handle the case where DoctorID is not provided in the query string
                return BadRequest("DoctorID not provided in the query string.");
            }

            string id = Request.Query["id"];


            try
            {
                string connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM Doctor WHERE DoctorID = @id";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UpdateDoctor.DoctorID = id;
                                UpdateDoctor.DoctorName = reader.GetString(1);
                                UpdateDoctor.LicenseNumber = reader.GetString(2);
                                UpdateDoctor.YearsOfExperience = reader.GetInt32(3);
                                UpdateDoctor.Education = reader.GetString(4);
                                UpdateDoctor.AvailabilitySchedule = reader.GetString(5);
                                UpdateDoctor.Specialization = reader.GetString(6);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the doctor data.");
                return Page();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Doctor SET LicenseNumber = @LicenseNumber, YearsOfExperience = @YearsOfExperience, " +
                                         "Education = @Education, AvailabilitySchedule = @AvailabilitySchedule, Specialization = @Specialization " +
                                         "WHERE DoctorID = @DoctorID";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@DoctorID", Request.Form["DoctorID"].ToString());
                        updateCommand.Parameters.AddWithValue("@LicenseNumber", Request.Form["LicenseNumber"].ToString());
                        updateCommand.Parameters.AddWithValue("@YearsOfExperience", Request.Form["YearsOfExperience"].ToString());
                        updateCommand.Parameters.AddWithValue("@Education", Request.Form["Education"].ToString());
                        updateCommand.Parameters.AddWithValue("@AvailabilitySchedule", Request.Form["AvailabilitySchedule"].ToString());
                        updateCommand.Parameters.AddWithValue("@Specialization", Request.Form["Specialization"].ToString());

                        updateCommand.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Doctor/Index");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while updating the doctor data.");
                return Page();
            }
        }
    }
}
