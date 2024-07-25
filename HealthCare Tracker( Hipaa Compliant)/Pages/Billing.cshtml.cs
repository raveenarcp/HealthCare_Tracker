using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

public class BillingModel : PageModel
{
    private readonly IConfiguration _configuration;

    public BillingModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public List<Bill> BillingRecords { get; set; }

    public IActionResult OnGet()
    {
        LoadBillingRecords();
        return Page();
    }

    public IActionResult OnPostRefresh()
    {
        // Call the stored procedure to insert billing data
        CallInsertBillingDataStoredProcedure();

        // Reload all billing records from the database
        LoadBillingRecords();

        return Page();
    }

    private void LoadBillingRecords()
    {
        BillingRecords = new List<Bill>();
        string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Bill", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = new Bill
                        {
                            BillID = reader.GetInt32(0),
                            BillingAmount = reader.GetDecimal(1),
                            DateIssued = reader.GetDateTime(2),
                            Status = reader.GetString(3),
                            VisitID = reader.GetString(4)
                        };

                        BillingRecords.Add(record);
                    }
                }
            }
        }
    }

    private void CallInsertBillingDataStoredProcedure()
    {
        string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("InsertBillingData", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add parameters if needed
                // command.Parameters.AddWithValue("@ParameterName", parameterValue);

                command.ExecuteNonQuery();
            }
        }
    }
}

public class Bill
{
    public int BillID { get; set; }
    public decimal BillingAmount { get; set; }
    public DateTime DateIssued { get; set; }
    public string Status { get; set; }
    public string VisitID { get; set; }
}
