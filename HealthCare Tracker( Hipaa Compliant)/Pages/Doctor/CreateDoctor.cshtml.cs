using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace HealthCare.Pages.Doctor
{
    public class CreateDoctorModel : PageModel
    {
        public void OnGet()
        {
        }
        public DoctorData NewDoctor = new DoctorData();
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string maxDoctorIDQuery = "SELECT MAX(DoctorID) FROM Doctor";
                    string maxDoctorIdResult = "";

                    using (SqlCommand maxIdCommand = new SqlCommand(maxDoctorIDQuery, connection))
                    {
                        maxDoctorIdResult = maxIdCommand.ExecuteScalar().ToString();
                    }

                    // Use the SQL Server function to generate the new DoctorID
                    string newDoctorIdQuery = $"SELECT dbo.GenerateNewPrimaryKey('{maxDoctorIdResult}', 'DOC0')";
                    string newDoctorIdResult = "";
                    using (SqlCommand newIdCommand = new SqlCommand(newDoctorIdQuery, connection))
                    {
                        newDoctorIdResult = newIdCommand.ExecuteScalar().ToString();
                        // Now newDoctorIdResult contains the updated DoctorID
                    }

                    NewDoctor.DoctorID = newDoctorIdResult;
                    NewDoctor.DoctorName = Request.Form["DoctorName"];
                    NewDoctor.LicenseNumber = Request.Form["LicenseNumber"];
                    NewDoctor.YearsOfExperience = Convert.ToInt32(Request.Form["YearsOfExperience"]);
                    NewDoctor.Education = Request.Form["Education"];
                    NewDoctor.AvailabilitySchedule = Request.Form["AvailabilitySchedule"];
                    NewDoctor.Specialization = Request.Form["Specialization"];

                    // Use parameterized query to prevent SQL injection
                    string query = "INSERT INTO Doctor (DoctorID,DoctorName, LicenseNumber, YearsOfExperience, Education, AvailabilitySchedule, Specialization) " +
                                   "VALUES (@DoctorID, @DoctorName,@LicenseNumber, @YearsOfExperience, @Education, @AvailabilitySchedule, @Specialization)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Use the calculated new DoctorID
                        command.Parameters.AddWithValue("@DoctorID", NewDoctor.DoctorID);
                        command.Parameters.AddWithValue("@DoctorName", NewDoctor.DoctorName);
                        command.Parameters.AddWithValue("@LicenseNumber", NewDoctor.LicenseNumber);
                        command.Parameters.AddWithValue("@YearsOfExperience", NewDoctor.YearsOfExperience);
                        command.Parameters.AddWithValue("@Education", NewDoctor.Education);
                        command.Parameters.AddWithValue("@AvailabilitySchedule", NewDoctor.AvailabilitySchedule);
                        command.Parameters.AddWithValue("@Specialization", NewDoctor.Specialization);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Doctor/Index"); // Redirect to the doctor list page after successful submission
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while saving the doctor data.");
                return Page();
            }

        }
    }
    }
