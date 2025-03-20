using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Attendance
{
    public class AddAttendanceRequest
    {

        [Required]
        public int EmployeeId { get; set; }
    }
}
