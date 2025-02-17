using System.Text.RegularExpressions;

namespace SyncroForge.Requests.Comany
{
    public class GetInvitationsRequest
    {
        private static readonly Regex DigitRegex = new Regex(@"^\d+$"); // Matches only digits

        public int StartAt { get; set; }
        public int Limit { get; set; }

        // ✅ Parameterless constructor for ASP.NET model binding
        public GetInvitationsRequest()
        {
            StartAt = 0;
            Limit = 50;
        }

        // ✅ Optional: Constructor that manually converts string inputs
        public GetInvitationsRequest(string startAt, string limit)
        {
            StartAt = IsValidNumber(startAt) ? int.Parse(startAt) : 0;
            Limit = IsValidNumber(limit) ? int.Parse(limit) : 50;
        }

        // Helper method to check if a string is a valid number
        private static bool IsValidNumber(string input)
        {
            return !string.IsNullOrEmpty(input) && DigitRegex.IsMatch(input);
        }
    }
}
