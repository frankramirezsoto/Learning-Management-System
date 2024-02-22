using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanvasLMS.Models.Entities
{
    [Index(nameof(Email))]
    public class Professor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<CourseCycle> CourseCycles { get; } = [];
        public List<Class> Classes { get; set; }
    }
}
