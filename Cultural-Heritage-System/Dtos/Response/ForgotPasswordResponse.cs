using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ForgotPasswordResponse
{
   
    public DateTime ExpiresAt { get; set; }


    public bool Used { get; set; } = false;

   
    public int Attempts { get; set; } = 0;

   
    public string? CreatedIp { get; set; }
}
