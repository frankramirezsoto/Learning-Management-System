using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

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
            if (ModelState.IsValid)
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
                (bool Success, string Message) remove = await _enrollmentRepository.DeleteAsync(courseCycleId, studentId);
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

                groupDTO.CourseCycle = null;
                groupList.Add(groupDTO);
            }

            //A list of students will be passed to the ViewData for the dropdown with available students to be populated
            List<StudentViewModel> studentsList = new List<StudentViewModel>();

            //A list of enrollments obtains the students enrolled
            var enrollments = await _enrollmentRepository.GetAllByCourseCycleIdAsync(id);
            if (enrollments != null)
            {
                foreach (var enrollment in enrollments)
                {
                    var studentDTO = new StudentViewModel
                    {
                        Id = enrollment.StudentId,
                        FirstName = enrollment.Student.FirstName,
                        LastName = enrollment.Student.LastName,
                        Email = enrollment.Student.Email,
                    };
                    studentsList.Add(studentDTO);
                }
            }

            ViewData["Students"] = studentsList;


            return View(groupList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingGroups = await _groupRepository.GetAllByCourseCycleIdAsync(model.CourseCycleId);

                if (existingGroups != null)
                {
                    //Variable saves a count of the id of the groups that exist 
                    //If a group is deleted the count will consider the group with the latest id number 
                    var groupIdCount = 0;
                    foreach (var group in existingGroups)
                    {
                        if (group.Id > groupIdCount)
                        {
                            groupIdCount = group.Id;
                        }
                    }

                    var groupDTO = new Group { Id = groupIdCount + 1, CourseCycleId = model.CourseCycleId };
                    (bool Success, string Message) addGroup = await _groupRepository.AddAsync(groupDTO);

                    return Content(addGroup.Message);
                }
            }

            return Content("There was an issue creating the group");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int Id, int CourseCycleId) 
        {
            try 
            {
                var deleteResult = await _groupRepository.DeleteAsync(Id,CourseCycleId);
                return Content(deleteResult.Message);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString());
            }
            return Content("There was an issue deleting the group");
        }

        [HttpPost]
        public async Task<IActionResult> EditGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Gets the group in the database
                var existingGroup = await _groupRepository.GetByIdAsync(model.Id, model.CourseCycleId);

                if (existingGroup != null)
                {
                    //Model has a list of int that contains StudentIdson this group 
                    //If null, we delete everyone
                    if (model.StudentIds == null) 
                    {
                        try
                        {
                            // Create a copy of the students collection
                            var studentsCopy = existingGroup.Students.ToList();

                            // Iterate over the copy and remove students one by one
                            foreach (var studentInGroup in studentsCopy)
                            {
                                var deleteResult = await _groupRepository.RemoveStudentFromGroupAsync(existingGroup.Id, existingGroup.CourseCycleId, studentInGroup.Id);
                            }

                            return Content("200");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error removing students: {ex.Message}");
                            // Log the exception
                            return Content("Error occurred while removing students");
                        }
                    }
                    else // there's StudentIds on the list
                    {
                        //Checkds if student is in group or if its new
                        foreach (var studentInModel in model.StudentIds)
                        {
                            var studentIsInGroup = false;
                            var studentIsNewInGroup = true; // Assume the student is new until proven otherwise

                            // Check if the student exists in the existing group
                            foreach (var studentInGroup in existingGroup.Students)
                            {
                                if (studentInGroup.Id == studentInModel)
                                {
                                    studentIsInGroup = true;
                                    studentIsNewInGroup = false; // Student is found in the existing group
                                    break;
                                }
                            }

                            // If the student is not in the existing group, it's new and needs to be added
                            if (studentIsNewInGroup)
                            {
                                var add = await _groupRepository.AddStudentToGroupAsync(existingGroup.Id, existingGroup.CourseCycleId,studentInModel);
                            }
                        }

                        try
                        {
                            // Create a copy of the students collection
                            var studentsCopy = existingGroup.Students.ToList();

                            // Iterate over the copy and remove students one by one
                            foreach (var studentInGroup in studentsCopy)
                            {
                                var studentToBeDeleted = true; // Assume the student will be deleted until proven otherwise

                                // Check if the student exists in the model
                                foreach (var studentInModel in model.StudentIds)
                                {
                                    if (studentInModel == studentInGroup.Id)
                                    {
                                        studentToBeDeleted = false; // Student exists in the model, so don't delete
                                        break; // No need to continue searching
                                    }
                                }

                                // If the student is not found in the model, mark it for deletion
                                if (studentToBeDeleted)
                                {
                                    var deleteResult = await _groupRepository.RemoveStudentFromGroupAsync(existingGroup.Id, existingGroup.CourseCycleId, studentInGroup.Id);
                                    Console.WriteLine(deleteResult);
                                }
                            }

                            return Content("200");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error removing students: {ex.Message}");
                            // Log the exception
                            return Content("Error occurred while removing students");
                        }
                    }
                }
            }

            return Content("An error has occurred");
        }
    }
}
