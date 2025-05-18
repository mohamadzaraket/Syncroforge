using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class PaySalaryRequest
    {
        [Required]
        public List<String> employeesIdentifiers { get; set; }
        [Range(0.01, float.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public float amount { get; set; }
        public bool AreIdentifiersValidUuids(out List<string> invalidIdentifiers)
        {
            invalidIdentifiers = new List<string>();

            foreach (var id in employeesIdentifiers)
            {
                if (!Guid.TryParse(id, out _))
                {
                    invalidIdentifiers.Add(id);
                }
            }

            return invalidIdentifiers.Count == 0;
        }
    }
}
