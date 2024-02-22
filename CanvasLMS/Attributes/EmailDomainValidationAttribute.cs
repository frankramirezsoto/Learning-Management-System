using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EmailDomainValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string email)
            {
                return email.EndsWith("@ulacit.ed.cr", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
