using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace HealthCare.Pages.Doctor
{
    public class DcotorModel : PageModel
    {
        public List<DoctorData> doctorList=new List<DoctorData>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=DESKTOP-QOQD6ET;Initial Catalog=HealthCareManagementSystem;Integrated Security=True;Encrypt=False";
                using(SqlConnection connection = new SqlConnection(connectionString)) 
                {
                    connection.Open();
                    String query = "Select doctorid,isnull(LicenseNumber,'')LicenseNumber,isnull(YearsOfExperience,0) YearsOfExperience,isnull(Education,'')Education,isnull(AvailabilitySchedule,'')AvailabilitySchedule,isnull(Specialization,'')Specialization,isnull(DoctorName,'')DoctorName from Doctor";
                    using (SqlCommand command = new SqlCommand( query, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader()) 
                        { 
                            while(reader.Read())
                            {
                                DoctorData d = new DoctorData();
                                d.DoctorID  = reader.GetString(0);
                                d.LicenseNumber = reader.GetString(1);
                                d.YearsOfExperience = reader.GetInt32(2);
                                d.Education = reader.GetString(3);
                                d.AvailabilitySchedule = reader.GetString(4);
                                d.Specialization = reader.GetString(5);
                                d.DoctorName = reader.GetString(6);
                                doctorList.Add(d);

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
    public class DoctorData
    {
       

        public string DoctorID;
        public string LicenseNumber;
        public int YearsOfExperience;
        public string Education;
        public string AvailabilitySchedule;
        public string Specialization;
        public string DoctorName;



    }

}
