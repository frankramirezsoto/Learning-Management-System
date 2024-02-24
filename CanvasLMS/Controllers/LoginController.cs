using CanvasLMS.Attributes;
using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CanvasLMS.Controllers
{
    public class LoginController : Controller
    {
        //Uses Professor and Student Repositories
        private readonly IProfessorRepository _professorRepository;
        private readonly IStudentRepository _studentRepository;

        public LoginController(IProfessorRepository professorRepository, IStudentRepository studentRepository)
        {
            _professorRepository = professorRepository;
            _studentRepository = studentRepository;
        }
        [RedirectLogged]
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

                //Verifies if the user logging is a Professor or a Student and sets the Session to an Object variable 
                //There are 2 Sessions, one for Student and one for Professor 
                //The SessionViewModel is used to store the current user's Id and Name 

                if (professor != null && VerifyPassword(model.Password, professor.Password))
                {
                    var session = new SessionViewModel { UserId = professor.Id, UserName = $"{professor.FirstName} {professor.LastName}" };
                    HttpContext.Session.SetObject("Professor", session);
                    // Redirect to Dashboard
                    return RedirectToAction("Index", "CourseCycle");
                }
                else if (student != null && VerifyPassword(model.Password, student.Password))
                {
                    var session = new SessionViewModel { UserId = student.Id, UserName = $"{student.FirstName} {student.LastName}" };
                    HttpContext.Session.SetObject("Student", session);
                    // Redirect to Dashboard
                    return RedirectToAction("Index", "CourseCycle");
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
        [HttpGet]
        public async Task<IActionResult> SignOut() 
        {
            // Clear the user's session data
            HttpContext.Session.Remove("Professor");
            HttpContext.Session.Remove("Student");

            return RedirectToAction("Index", "Home");
        }
    }
}
