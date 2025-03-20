using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Attendance
{
    public class GetAttendanceCompanyRequest
    {

        [Required]
        public string CompanyIdentifier { get; set; }
    }
}
