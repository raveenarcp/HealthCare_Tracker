// Pages/Payment.cshtml.cs

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

public class PaymentModel : PageModel
{
    private readonly IConfiguration _configuration;

    public PaymentModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public List<Payment> PaymentRecords { get; set; }

    public IActionResult OnGet()
    {
        LoadPaymentRecords();
        return Page();
    }

    public IActionResult OnPostRefresh()
    {
        // Call the stored procedure to insert payment data
        CallInsertPaymentDataStoredProcedure();

        // Reload all payment records from the database
        LoadPaymentRecords();

        return Page();
    }

    private void LoadPaymentRecords()
    {
        System.Diagnostics.Debug.WriteLine("Test");
        PaymentRecords = new List<Payment>();
        string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("SELECT PaymentID, BillID, isnull(PlanID, '') PlanID, isnull(TotalAmount, 0)TotalAmount," +
                "PaymentDate, isnull(AmountCoveredByInsurance, 0)AmountCoveredByInsurance, isnull(AmountToBePaid, 0)AmountToBePaid FROM Payment", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Payment p = new Payment();
                        p.PaymentID = reader.GetInt32(0);
                        p.BillID = reader.GetInt32(1);
                        p.PlanID = reader.GetString(2);
                        p.TotalAmount = reader.GetDecimal(3);
                        p.PaymentDate = reader.GetDateTime(4);
                        p.AmountCoveredByInsurance = reader.GetDecimal(5);
                        p.AmountToBePaid = reader.GetDecimal(6);
                        
                        PaymentRecords.Add(p);
                    }
                }
            }
        }
    }

    private void CallInsertPaymentDataStoredProcedure()
    {
        string connectionString = _configuration.GetConnectionString("HealthCareDatabase");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("InsertPaymentData", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Add parameters if needed
                // command.Parameters.AddWithValue("@ParameterName", parameterValue);
                command.ExecuteNonQuery();
            }
        }
    }
}

public class Payment
{
    public int PaymentID { get; set; }
    public int BillID { get; set; }
    public string? PlanID { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal? AmountCoveredByInsurance { get; set; }
    public decimal? AmountToBePaid { get; set; }
}
