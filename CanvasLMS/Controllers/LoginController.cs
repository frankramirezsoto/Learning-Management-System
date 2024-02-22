using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CanvasLMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly IStudentRepository _studentRepository;

        public LoginController(IProfessorRepository professorRepository, IStudentRepository studentRepository)
        {
            _professorRepository = professorRepository;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var professor = await _professorRepository.GetByEmailAsync(model.Email);
                var student = await _studentRepository.GetByEmailAsync(model.Email);

                if (professor != null && VerifyPassword(model.Password, professor.Password))
                {

                    HttpContext.Session.SetObject("Professor", professor);
                    // Redirect to Dashboard
                    return RedirectToAction("Dashboard", "Home");
                }
                else if (student != null && VerifyPassword(model.Password, student.Password))
                {
                    HttpContext.Session.SetObject("Student", student);
                    // Redirect to Dashboard
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                }
            }

            return View(model);
        }
        //Verify Password function can be further worked to check tokenization 
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }
    }

}
