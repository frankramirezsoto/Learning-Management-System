using CanvasLMS.Attributes;
using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Controllers
{
    [RequireSession]
    public class CourseCycleController : MainCourseCycleController
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICareerRepository _careerRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly ICycleRepository _cycleRepository;
        private readonly IGroupRepository _groupRepository;

        public CourseCycleController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseCycleRepository courseCycleRepository, ICourseRepository courseRepository, 
            ICareerRepository careerRepository, IProfessorRepository professorRepository, ICycleRepository cycleRepository, IGroupRepository groupRepository) : base(enrollmentRepository,studentRepository,courseCycleRepository)
        { 
            _courseRepository = courseRepository;
            _careerRepository = careerRepository;
            _professorRepository = professorRepository;
            _cycleRepository = cycleRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //Used to pass data to partial view at the index level, create coursecycle will open as modal and requires this selectlist
            var careers = await _careerRepository.GetAllAsync();
            ViewData["Careers"] = new SelectList(careers, "Id", "Name");
            ViewData["Courses"] = new SelectList(new List<Course>(), "Id", "Name");
            ViewData["Professors"] = new SelectList(new List<ProfessorViewModel>(), "Id", "FullName");

            //Gets the session to be passed to the View and handle the permissions on this view
            var professorSession = HttpContext.Session.GetObject<SessionViewModel>("Professor");
            var studentSession = HttpContext.Session.GetObject<SessionViewModel>("Student");

            ViewBag.Professor = professorSession;
            ViewBag.Student = studentSession;
            //List or CourseCycle
            IEnumerable<CourseCycle> courseCyclesList = new List<CourseCycle>();
            if (professorSession != null) 
            {
                courseCyclesList = await _courseCycleRepository.GetAllByProfessorIdAsync(professorSession.UserId);
            } else if (studentSession != null)
            {
                courseCyclesList = await _courseCycleRepository.GetAllByStudentIdAsync(studentSession.UserId);
            }
            //Convert List to CourseCycleViewModel
            List<CourseCycleViewModel> courseCycleDTOList = new List<CourseCycleViewModel>();
            foreach (var courseCycle in courseCyclesList) 
            {
                var coursecycleDTO = new CourseCycleViewModel();
                ObjectMapper.MapProperties(courseCycle, coursecycleDTO);
                courseCycleDTOList.Add(coursecycleDTO);
            }
            return View(courseCycleDTOList);
        }

        [HttpGet]
        // GET: CourseCycles/Course/5
        public async Task<IActionResult> Course(int id)
        {
            //Gets the session to be passed to the View and handle the permissions on this view
            var professorSession = HttpContext.Session.GetObject<SessionViewModel>("Professor");
            var studentSession = HttpContext.Session.GetObject<SessionViewModel>("Student");
            if (studentSession != null)
            {
                //Checks with the parent Controller function that the Student is part of the screen being requested 
                var studentIsInCourse = await StudentIsInCourse(id);
                if (!studentIsInCourse)
                {
                    return RedirectToAction("NotAuthorized", "Home");
                }
            }

            ViewBag.Professor = professorSession;
            ViewBag.Student = studentSession;
            ViewData["CourseCycleId"] = id; //Used to pass ViewData to NavBar Partial Views
            var courseCycle = await _courseCycleRepository.GetByIdAsync(id);

            if (courseCycle == null)
            {
                return NotFound();
            }

            var coursecycleDTO = new CourseCycleViewModel();
            ObjectMapper.MapProperties(courseCycle, coursecycleDTO);

            return View(coursecycleDTO);
        }

        // GET: CourseCycle/Create
        public async Task<IActionResult> Create()
        {
            return PartialView("_Create");
        }

        // POST: CourseCycles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,ProfessorId,DayOfWeek,TimeOfDay,maxQuota,MeetingUrl,Color")] CourseCycleViewModel courseCycle)
        {
            if (ModelState.IsValid)
            {
                var currentCycle = await _cycleRepository.GetCurrentCycleAsync();
                var existingCourseCycles = await _courseCycleRepository.GetAllByCourseIdAndCycleId(courseCycle.CourseId, currentCycle.Id);
                
                var courseCycleDTO = new CourseCycle 
                { 
                    CourseId = courseCycle.CourseId,
                    CycleId = currentCycle.Id,
                    GroupNo = existingCourseCycles.Count()+1,
                    ProfessorId = courseCycle.ProfessorId,
                    DayOfWeek = courseCycle.DayOfWeek,
                    TimeOfDay = courseCycle.TimeOfDay,
                    maxQuota = courseCycle.maxQuota,
                    MeetingUrl = courseCycle.MeetingUrl,
                    Color = courseCycle.Color
                };
                await _courseCycleRepository.AddAsync(courseCycleDTO);
                return Content("200");
            }

            return Content("There was an issue adding the course. Please check the fields.");
        }

        //Methods used to populate dropdowns based on inputs
        [HttpPost]
        public async Task<IActionResult> GetCoursesByCareerId(int careerId)
        {
            var courses = await _courseRepository.GetCoursesByCareerIdAsync(careerId);

            var courseList = new SelectList(courses, "Id", "Name");
            return Json(courseList);
        }
        [HttpPost]
        public async Task<IActionResult> GetProfessorsByCareerId(int careerId)
        {
            var professors = await _professorRepository.GetProfessorsByCareer(careerId);
            //Convert List to ProfessorViewModel
            List<ProfessorViewModel> professorList = new List<ProfessorViewModel>();
            foreach (var professor in professors)
            {
                var professorDTO = new ProfessorViewModel();
                ObjectMapper.MapProperties(professor, professorDTO);
                professorList.Add(professorDTO);
            }

            var professorSelectList = new SelectList(professorList, "Id", "IdFullName");

            return Json(professorSelectList);
        }

    }
}
