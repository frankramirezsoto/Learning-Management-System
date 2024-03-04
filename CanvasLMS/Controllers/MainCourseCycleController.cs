using CanvasLMS.Extensions;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CanvasLMS.Controllers
{
    public class MainCourseCycleController : Controller
    {
        protected readonly IEnrollmentRepository _enrollmentRepository;
        protected readonly IStudentRepository _studentRepository;
        protected readonly ICourseCycleRepository _courseCycleRepository;

        public MainCourseCycleController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseCycleRepository courseCycleRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseCycleRepository = courseCycleRepository;
        }

        public async Task<bool> StudentIsInCourse(int courseCycleId) 
        {
            //Gets the studeent current session
            var studentSession = HttpContext.Session.GetObject<SessionViewModel>("Student");

            //Gets the CourseCycle to pass ViewData to be used by the banner 
            var courseCycle = await _courseCycleRepository.GetByIdAsync(courseCycleId);

            //Gets the enrollments for the courseCycleId
            var enrollments = await _enrollmentRepository.GetAllByCourseCycleIdAsync(courseCycle.Id);

            //Checks if student trying to access is enrolled to this coursecycle 
            if (studentSession != null)
            {
                var studentLogged = await _studentRepository.GetByIdAsync(studentSession.UserId);
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Student == studentLogged)
                    {
                        return true;
                    }
                }
            }
            //Is student wasn't found returns null
            return false;
        }
    }
}
