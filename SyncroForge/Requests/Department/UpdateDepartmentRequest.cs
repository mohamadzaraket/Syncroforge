using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Department
{
    public class UpdateDepartmentRequest
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Identifier { get; set; }

        public IFormFile? Logo { get; set; } // ✅ Nullable now

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Logo != null) // ✅ Only validate if a file is provided
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

                var fileExtension = Path.GetExtension(Logo.FileName)?.ToLower();
                var fileMimeType = Logo.ContentType.ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    yield return new ValidationResult($"Invalid file type. Allowed extensions: {string.Join(", ", allowedExtensions)}", new[] { nameof(Logo) });
                }

                if (!allowedMimeTypes.Contains(fileMimeType))
                {
                    yield return new ValidationResult($"Invalid file type. Allowed MIME types: {string.Join(", ", allowedMimeTypes)}", new[] { nameof(Logo) });
                }
            }
        }


    }
}
