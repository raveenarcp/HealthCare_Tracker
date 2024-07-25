using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace HealthCare.Pages.Patient
{
    public class IndexModel : PageModel
    {
        public List<PatientData> patientList=new List<PatientData>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";
                using(SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    String query = "SELECT PatientID, FirstName ,LastName ,DateOfBirth ,Gender,Email,Address ,PhoneNumber ,EmergencyContact,BloodType FROM Patient";
                    using (SqlCommand command = new SqlCommand( query, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader()) 
                        { 
                            while(reader.Read())
                            {
                                PatientData p = new PatientData();
                                p.PatientID = reader.GetString(0);
                                
                                p.FirstName = reader.GetString(1);
                                p.LastName = reader.GetString(2);
                                p.DateOfBirth = reader.GetDateTime(3).ToString();
                                p.Gender = reader.GetString(4);
                                p.Email = reader.GetString(5);
                                p.Address = reader.GetString(6);
                                p.PhoneNumber = reader.GetString(7);
                                p.EmergencyContact = reader.GetString(8);
                                p.BloodType = reader.GetString(9);
                                patientList.Add(p);
                               
                            }
                        }
                    }
                
                
                }
                
            }
            catch(Exception ex)
            {

                Console.WriteLine("Exception is :" + ex.ToString());
            }
            
        }
    }
    public class PatientData
    {
        public string PatientID;
        public byte[] ssnbyte;
        public string SSN;
        public string FirstName;
        public string LastName         ;
        public string DateOfBirth      ;
        public string Gender           ;
        public string Email            ;
        public string Address          ;
        public string PhoneNumber      ;
        public string EmergencyContact ;
        public string BloodType        ;
    }

}
