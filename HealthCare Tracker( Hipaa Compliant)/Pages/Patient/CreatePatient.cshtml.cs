using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace HealthCare.Pages.Patient
{
    public class CreatePatientModel : PageModel
    {
        public PatientData NewPatient = new PatientData();
        public void OnGet()
        {
        }

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
                    string maxPatientIDQuery = "SELECT MAX(PatientID) FROM Patient";
                    string maxPatientIdResult = "";

                    using (SqlCommand maxIdCommand = new SqlCommand(maxPatientIDQuery, connection))
                    {
                        maxPatientIdResult = maxIdCommand.ExecuteScalar().ToString();
                    }

                    // Use the SQL Server function to generate the new PatientID
                    string newPatientIdQuery = $"SELECT dbo.GenerateNewPrimaryKey('{maxPatientIdResult}', 'PAT0')";
                    string newPatientIdResult = "";
                    using (SqlCommand newIdCommand = new SqlCommand(newPatientIdQuery, connection))
                    {
                         newPatientIdResult = newIdCommand.ExecuteScalar().ToString();
                        // Now newPatientIdResult contains the updated PatientID
                    }

                    NewPatient.PatientID = newPatientIdResult;
                    NewPatient.ssnbyte = Encoding.UTF8.GetBytes(Request.Form["SSN"]);

                    NewPatient.FirstName = Request.Form["FirstName"];
                    NewPatient.LastName = Request.Form["LastName"];
                    NewPatient.DateOfBirth = Request.Form["DateOfBirth"];
                    NewPatient.Gender = Request.Form["Gender"];
                    NewPatient.Email = Request.Form["Email"];
                    NewPatient.Address = Request.Form["Address"];
                    NewPatient.PhoneNumber = Request.Form["PhoneNumber"];
                    NewPatient.EmergencyContact = Request.Form["EmergencyContact"];
                    NewPatient.BloodType = Request.Form["BloodType"];

                    // Use parameterized query to prevent SQL injection
                    string query = "INSERT INTO Patient (PatientID, SSN, FirstName, LastName, DateOfBirth, Gender, Email, Address, PhoneNumber, EmergencyContact, BloodType) " +
                                   "VALUES (@PatientID,Convert(Binary(16), @SSN), @FirstName, @LastName, @DateOfBirth, @Gender, @Email, @Address, @PhoneNumber, @EmergencyContact, @BloodType)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Use the calculated new PatientID
                        command.Parameters.AddWithValue("@PatientID", NewPatient.PatientID);
                        command.Parameters.AddWithValue("@SSN", NewPatient.ssnbyte);
                        command.Parameters.AddWithValue("@FirstName", NewPatient.FirstName);
                        command.Parameters.AddWithValue("@LastName", NewPatient.LastName);
                        command.Parameters.AddWithValue("@DateOfBirth", NewPatient.DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", NewPatient.Gender);
                        command.Parameters.AddWithValue("@Email", NewPatient.Email);
                        command.Parameters.AddWithValue("@Address", NewPatient.Address);
                        command.Parameters.AddWithValue("@PhoneNumber", NewPatient.PhoneNumber);
                        command.Parameters.AddWithValue("@EmergencyContact", NewPatient.EmergencyContact);
                        command.Parameters.AddWithValue("@BloodType", NewPatient.BloodType);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Patient/Index"); // Redirect to the patient list page after successful submission
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while saving the patient data.");
                return Page();
            }
        }

    }
}
