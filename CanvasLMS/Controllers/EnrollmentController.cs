using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CanvasLMS.Controllers
{
    public class EnrollmentController : MainCourseCycleController
    {
        private readonly IGroupRepository _groupRepository;
        public EnrollmentController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, 
            ICourseCycleRepository courseCycleRepository, IGroupRepository groupRepository) : base(enrollmentRepository, studentRepository, courseCycleRepository) 
        {
            _groupRepository = groupRepository;
        }

        public async Task<IActionResult> Students(int id)//refers to the CourseCycleid
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
            //Passes course cycle id to be used when adding the enrollment
            ViewData["CourseCycleId"] = id;

            //Gets the CourseCycle to pass ViewData to be used by the banner 
            var courseCycle = await _courseCycleRepository.GetByIdAsync(id);
            ViewData["CourseColor"] = courseCycle.Color;
            ViewData["CourseName"] = courseCycle.Course.Name;

            //Gets the enrollments for the courseCycleId
            var enrollments = await _enrollmentRepository.GetAllByCourseCycleIdAsync(id);

            //Converts list of enrollments into a list of enrollmentviewmodels
            var enrollmentsList = new List<EnrollmentViewModel>();
            foreach (var enrollment in enrollments) 
            {
                var enrollmentDTO = new EnrollmentViewModel();
                ObjectMapper.MapProperties(enrollment, enrollmentDTO);
                enrollmentsList.Add(enrollmentDTO);
            }
            //A list of students is sent so the Partial View _CreateEnrollment can be populated with the available Students
            var students = await _studentRepository.GetAllAsync();
            //Convert List to StudentViewModel
            List<StudentViewModel> studentsList = new List<StudentViewModel>();
            foreach (var student in students)
            {
                var studentDTO = new StudentViewModel();
                ObjectMapper.MapProperties(student, studentDTO);
                studentsList.Add(studentDTO);
            }

            var studentsSelectList = new SelectList(studentsList, "Id", "IdFullName");

            ViewData["Students"] = studentsSelectList;

            return View(enrollmentsList);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEnrollment() 
        { 
            return View();
        }
        //This controller will be used by a Partial View displayed on a modal
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(EnrollmentViewModel enrollment)
        {
            if(ModelState.IsValid) 
            {
                //Looks for the courseCycle to get the Max Quota accepted 
                var courseCycle = await _courseCycleRepository.GetByIdAsync(enrollment.CourseCycleId);
                if (courseCycle != null)
                {
                    if (courseCycle.maxQuota <= courseCycle.Enrollments.Count) 
                    {
                        return Content($"This course allows a maximum of {courseCycle.maxQuota} students to be enrolled.");
                    }
                }
                int studentId = 0; //This variable will change according to the validation 

                var studentById = await _studentRepository.GetByIdAsync(enrollment.Student.Id);//Get Student by id 
                var studentByEmail = await _studentRepository.GetByEmailAsync(enrollment.Student.Email);//Get Student by email 
                //Validations added to see if student has to be added or if it exists 
                if (studentByEmail == null && studentById == null)
                {
                    await _studentRepository.AddNewStudent(enrollment.Student);

                    studentId = enrollment.Student.Id;
                }
                else if (studentByEmail != null && studentById != null)
                {
                    if (studentById.Id == studentByEmail.Id)
                    {
                        studentId = enrollment.Student.Id;
                    }
                    else
                    {
                        return Content("This email already exists for another student with a different identification. Check the identification or email field.");
                    }
                }
                else if (studentByEmail != null && studentById == null) 
                {
                    return Content("Email exists for another identification.");
                }
                else if (studentByEmail == null && studentById != null)
                {
                    return Content("Identification exists for another email.");
                }

                var enrollmentDTO = new Enrollment { CourseCycleId = enrollment.CourseCycleId, StudentId = studentId };

                (bool Sucess, string Message) add = await _enrollmentRepository.AddAsync(enrollmentDTO);

                return Content(add.Message);
            }
            return Content("Unable to enroll student. Please try again.");
        }

        //Method used to populate input fields based on dropdown
        [HttpPost]
        public async Task<IActionResult> GetStudentById(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);

            if (student != null)
            {
                var studentDTO = new StudentViewModel { Id = student.Id, FirstName = student.FirstName, LastName = student.LastName, Email = student.Email };
                return Json(studentDTO);
            }

            return Json(null);
        }

        //Method to Delete a Student Enrollment 
        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int courseCycleId, int studentId) 
        { 
            var enrollment = await _enrollmentRepository.GetByCompositeKeysAsync(courseCycleId, studentId);
            if (enrollment != null)
            {
                (bool Success, string Message) remove = await _enrollmentRepository.DeleteAsync(courseCycleId,studentId);
                if (remove.Success) 
                {
                    return Content("200");
                } 
                else 
                {
                    return Content(remove.Message);
                }
            }
            return Content("Unable to remove student enrollment. Please try again.");
        }

        //-----------------------------------------------------------------------------------------------
        //Groups Controller 

        public async Task<IActionResult> Groups(int id)//refers to the CourseCycleid
        {
            //Gets the session to be passed to the View and handle the permissions on this view
            var professorSession = HttpContext.Session.GetObject<SessionViewModel>("Professor");
            var studentSession = HttpContext.Session.GetObject<SessionViewModel>("Student");
            //if (studentSession != null)
            //{
            //    //Checks with the parent Controller function that the Student is part of the screen being requested 
            //    var studentIsInCourse = await StudentIsInCourse(id);
            //    if (!studentIsInCourse)
            //    {
            //        return RedirectToAction("NotAuthorized", "Home");
            //    }
            //}

            ViewBag.Professor = professorSession;
            ViewBag.Student = studentSession;
            //Passes course cycle id to be used when adding the group
            ViewData["CourseCycleId"] = id;

            //Gets the CourseCycle to pass ViewData to be used by the banner 
            var courseCycle = await _courseCycleRepository.GetByIdAsync(id);
            ViewData["CourseColor"] = courseCycle.Color;
            ViewData["CourseName"] = courseCycle.Course.Name;

            //Gets the enrollments for the courseCycleId
            var groups = await _groupRepository.GetAllByCourseCycleIdAsync(id);

            //Converts list of groups into a list of groupviewmodels
            var groupList = new List<GroupViewModel>();
            foreach (var group in groups)
            {
                var groupDTO = new GroupViewModel();
                ObjectMapper.MapProperties(group, groupDTO);
                groupList.Add(groupDTO);
            }
            //A list of students will be passed to the ViewData for the dropdown with available students to be populated
            List<StudentViewModel> studentsList = new List<StudentViewModel>();
            
            //A list of enrollments is sent so the Partial View _CreateGroup can be populated with the available Students
            var enrollments = await _enrollmentRepository.GetAllByCourseCycleIdAsync(id);
            if (enrollments != null) 
            {
                foreach (var enrollment in enrollments)
                {
                    var studentDTO = new StudentViewModel();
                    ObjectMapper.MapProperties(enrollment.Student, studentDTO);
                    studentsList.Add(studentDTO);
                }
            }

            var studentsSelectList = new SelectList(studentsList, "Id", "IdFullName");

            ViewData["Students"] = studentsSelectList;

            return View(groupList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupViewModel model) 
        {
            if(ModelState.IsValid)
            {
                var existingGroups = await _groupRepository.GetAllByCourseCycleIdAsync(model.CourseCycleId);
                var groupCount = 0;
                if (existingGroups != null)
                {
                    groupCount = existingGroups.Count();
                }
                var groupDTO = new Group { Id=groupCount+1, CourseCycleId=model.CourseCycleId };
                (bool Success, string Message) addGroup = await _groupRepository.AddAsync(groupDTO);

                return Content(addGroup.Message);
            }

            return Content("There was an issue creating the group");
        }

    }
}
