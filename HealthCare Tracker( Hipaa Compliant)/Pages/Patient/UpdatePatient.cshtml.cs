using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Text;

namespace HealthCare.Pages.Patient
{
    public class UpdatePatientModel : PageModel
    {
        public PatientData UpdatePatient = new PatientData();

        public IActionResult OnGet()
        {
            if (!Request.Query.ContainsKey("id"))
            {
                // Handle the case where PatientID is not provided in the query string
                return BadRequest("PatientID not provided in the query string.");
            }

            string id = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT PatientID, FirstName ,LastName ,DateOfBirth ,Gender,Email,Address ,PhoneNumber ,EmergencyContact,BloodType FROM Patient WHERE PatientID = @id";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UpdatePatient.PatientID = id;
                              
                                UpdatePatient.FirstName = reader.GetString(1);
                                UpdatePatient.LastName = reader.GetString(2);
                                UpdatePatient.DateOfBirth = reader.GetDateTime(3).ToString();
                                UpdatePatient.Gender = reader.GetString(4);
                                UpdatePatient.Email = reader.GetString(5);
                                UpdatePatient.Address = reader.GetString(6);
                                UpdatePatient.PhoneNumber = reader.GetString(7);
                                UpdatePatient.EmergencyContact = reader.GetString(8);
                                UpdatePatient.BloodType = reader.GetString(9);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the patient data.");
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

                    string updateQuery = "UPDATE Patient SET  FirstName = @FirstName, " +
                                         "LastName = @LastName, DateOfBirth = @DateOfBirth, Gender = @Gender, " +
                                         "Email = @Email, Address = @Address, PhoneNumber = @PhoneNumber, " +
                                         "EmergencyContact = @EmergencyContact, BloodType = @BloodType " +
                                         "WHERE PatientID = @PatientID";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@PatientID", Request.Form["PatientID"].ToString());
                      
                        updateCommand.Parameters.AddWithValue("@FirstName", Request.Form["FirstName"].ToString());
                        updateCommand.Parameters.AddWithValue("@LastName", Request.Form["LastName"].ToString());
                        updateCommand.Parameters.AddWithValue("@DateOfBirth", Convert.ToDateTime(Request.Form["DateOfBirth"]));
                        updateCommand.Parameters.AddWithValue("@Gender", Request.Form["Gender"].ToString());
                        updateCommand.Parameters.AddWithValue("@Email", Request.Form["Email"].ToString());
                        updateCommand.Parameters.AddWithValue("@Address", Request.Form["Address"].ToString());
                        updateCommand.Parameters.AddWithValue("@PhoneNumber", Request.Form["PhoneNumber"].ToString());
                        updateCommand.Parameters.AddWithValue("@EmergencyContact", Request.Form["EmergencyContact"].ToString());
                        updateCommand.Parameters.AddWithValue("@BloodType", Request.Form["BloodType"].ToString());

                        updateCommand.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Patient/Index");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while updating the patient data.");
                return Page();
            }
        }
    }
}