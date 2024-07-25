using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace HealthCare.Pages.Admission
{
    public class UpdateAdmissionModel : PageModel
    {
        public AdmissionRecord UpdateAdmission = new AdmissionRecord();

        public IActionResult OnGet()
        {
            if (!Request.Query.ContainsKey("id"))
            {
                // Handle the case where AdmissionID is not provided in the query string
                return BadRequest("AdmissionID not provided in the query string.");
            }

            string id = Request.Query["id"];

            try
            {
                string connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";
                int admissionId = Convert.ToInt32(id);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT AdmissionID,TreatmentID,RoomID,AdmissionDate,Isnull(DischargeDate,'') DischargeDate,isnull(AdmissionReason,'')AdmissionReason,Isnull(Status,'')Status FROM Admission WHERE AdmissionID = @admissionId";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@AdmissionID", admissionId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UpdateAdmission.AdmissionID = admissionId;
                                UpdateAdmission.TreatmentID = reader.GetString(1);
                                UpdateAdmission.RoomID = reader.GetString(2);
                                UpdateAdmission.AdmissionDate = reader.GetDateTime(3);
                                
                                UpdateAdmission.DischargeDate = reader.GetDateTime(4);
                                UpdateAdmission.AdmissionReason = reader.GetString(5);
                                UpdateAdmission.Status = reader.GetString(6);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the admission data.");
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

                    string updateQuery = "UPDATE Admission SET TreatmentID = @TreatmentID, RoomID = @RoomID, " +
                                         "AdmissionDate = @AdmissionDate, DischargeDate = @DischargeDate, " +
                                         "AdmissionReason = @AdmissionReason, Status = @Status " +
                                         "WHERE AdmissionID = @AdmissionID";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@AdmissionID", Convert.ToInt32(Request.Form["AdmissionID"]));
                        updateCommand.Parameters.AddWithValue("@TreatmentID", Request.Form["TreatmentID"].ToString());
                        updateCommand.Parameters.AddWithValue("@RoomID", Request.Form["RoomID"].ToString());
                        updateCommand.Parameters.AddWithValue("@AdmissionDate", Convert.ToDateTime(Request.Form["AdmissionDate"]));
                        updateCommand.Parameters.AddWithValue("@DischargeDate", Request.Form["DischargeDate"] == "" ? DBNull.Value : (object)Convert.ToDateTime(Request.Form["DischargeDate"]));
                        updateCommand.Parameters.AddWithValue("@AdmissionReason", Request.Form["AdmissionReason"].ToString());
                        updateCommand.Parameters.AddWithValue("@Status", Request.Form["Status"].ToString());

                        updateCommand.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Admission/Admission");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while updating the admission data.");
                return Page();
            }
        }
    }
}
