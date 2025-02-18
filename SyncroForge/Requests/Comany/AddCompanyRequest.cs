using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace SyncroForge.Requests.Company
{
    public class AddCompanyRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile? CompanyLogo { get; set; } // ✅ Nullable now

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CompanyLogo != null) // ✅ Only validate if a file is provided
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

                var fileExtension = Path.GetExtension(CompanyLogo.FileName)?.ToLower();
                var fileMimeType = CompanyLogo.ContentType.ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    yield return new ValidationResult($"Invalid file type. Allowed extensions: {string.Join(", ", allowedExtensions)}", new[] { nameof(CompanyLogo) });
                }

                if (!allowedMimeTypes.Contains(fileMimeType))
                {
                    yield return new ValidationResult($"Invalid file type. Allowed MIME types: {string.Join(", ", allowedMimeTypes)}", new[] { nameof(CompanyLogo) });
                }
            }
        }
    }
}
