using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;

namespace CanvasLMS.Controllers
{
    public class AssignmentsController : MainCourseCycleController
    {
        private readonly IEvaluationTaskRepository _evaluationTaskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEvaluationItemRepository _evaluationItemRepository;
        private readonly ITaskSubmissionRepository _taskSubmissionRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AssignmentsController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseCycleRepository courseCycleRepository,
            IEvaluationTaskRepository evaluationTaskRepository, IGroupRepository groupRepository, IEvaluationItemRepository evaluationItemRepository, ITaskSubmissionRepository taskSubmissionRepository, IHostingEnvironment hostingEnvironment) : base(enrollmentRepository, studentRepository, courseCycleRepository)
        {
            _evaluationTaskRepository = evaluationTaskRepository;
            _groupRepository = groupRepository;
            _evaluationItemRepository = evaluationItemRepository;
            _taskSubmissionRepository = taskSubmissionRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Tasks(int id) //Refers to the CourseCycleId
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
            //Passes course cycle id to be used when adding the evaluation
            ViewData["CourseCycleId"] = id;

            //Gets the CourseCycle to pass ViewData to be used by the banner 
            var courseCycle = await _courseCycleRepository.GetByIdAsync(id);
            ViewData["CourseColor"] = courseCycle.Color;
            ViewData["CourseName"] = courseCycle.Course.Name;

            //Gets the evaluationItems for the courseCycleId - The evaluation item themselves contain their associated tasks
            var evaluationItems = await _evaluationItemRepository.GetAllByCourseCycleIdAsync(id);
            //Converts list of evaluationItems into a list of evaluationItemsviewmodels
            var evaluationItemsList = new List<EvaluationItemViewModel>();
            foreach (var evaluationItem in evaluationItems)
            {
                var evaluationItemDTO = new EvaluationItemViewModel();
                ObjectMapper.MapProperties(evaluationItem, evaluationItemDTO);
                evaluationItemsList.Add(evaluationItemDTO);
            }

            return View(evaluationItemsList);
        }

        //Method to add an Evaluation Item
        [HttpPost]
        public async Task<IActionResult> CreateTask(EvaluationTaskViewModel evaluationTask)
        {
            if (ModelState.IsValid)
            {
                var evaluationTaskDTO = new EvaluationTask();
                ObjectMapper.MapProperties(evaluationTask, evaluationTaskDTO);
                evaluationTaskDTO.Published = DateTime.Now;

                (bool Success, string Message) addTask = await _evaluationTaskRepository.AddAsync(evaluationTaskDTO);

                return Content(addTask.Message);
            }
            return Content("There was an issue adding the evaluation task.");
        }

        //Method to Update an Evaluation Task
        [HttpPost]
        public async Task<IActionResult> UpdateTask(EvaluationTaskViewModel evaluationTask)
        {
            if (ModelState.IsValid)
            {
                var taskDTO = new EvaluationTask();
                ObjectMapper.MapProperties(evaluationTask, taskDTO);
                (bool Success, string Message) updateItem = await _evaluationTaskRepository.UpdateAsync(taskDTO);

                return Content(updateItem.Message);
            }
            return Content("There was an issue adding the evaluation item.");
        }


        //Method to Delete an Evaluatoin Task
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int evaluationTaskId)
        {
            var evaluationTask = await _evaluationTaskRepository.GetByIdAsync(evaluationTaskId);
            if (evaluationTask != null)
            {
                (bool Success, string Message) remove = await _evaluationTaskRepository.DeleteAsync(evaluationTaskId);
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

        //-------------------------------------------------------------------------------------------------
        //Task View

        public async Task<IActionResult> Task(int courseCycleId, int taskId) 
        {
            //Gets the session to be passed to the View and handle the permissions on this view
            var professorSession = HttpContext.Session.GetObject<SessionViewModel>("Professor");
            var studentSession = HttpContext.Session.GetObject<SessionViewModel>("Student");
            if (studentSession != null)
            {
                //Checks with the parent Controller function that the Student is part of the screen being requested 
                var studentIsInCourse = await StudentIsInCourse(courseCycleId);
                if (!studentIsInCourse)
                {
                    return RedirectToAction("NotAuthorized", "Home");
                }
            }

            ViewBag.Professor = professorSession;
            ViewBag.Student = studentSession;
            //Passes course cycle id to be used when adding the evaluation
            ViewData["CourseCycleId"] = courseCycleId;

            //Gets the CourseCycle to pass ViewData to be used by the banner 
            var courseCycle = await _courseCycleRepository.GetByIdAsync(courseCycleId);
            ViewData["CourseColor"] = courseCycle.Color;
            ViewData["CourseName"] = courseCycle.Course.Name;

            //Will pass the groups as ViewData for the professor to use it 
            var groups = await _groupRepository.GetAllByCourseCycleIdAsync(courseCycleId);
            ViewData["Groups"] = groups;

            //Gets the task by its id
            var evaluationTask = await _evaluationTaskRepository.GetByIdAsync(taskId);
            //Converts task into TaskViewModel
            var taskViewModel = new EvaluationTaskViewModel();
            ObjectMapper.MapProperties(evaluationTask, taskViewModel);

            return View(taskViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAssignment(TaskSubmissionViewModel model, IFormFile fileUpload)
        {
            //Checks if file has been uploaded
            if (fileUpload == null || fileUpload.Length == 0)
            {
                return Content("Please select a file to upload.");
            }

            //Will look for the evaluation task and determine if it's Evaluation Item is groupal or individual 
            var taskIsGroupal = false;
            var evaluationTask = await _evaluationTaskRepository.GetByIdAsync(model.EvaluationTaskId);
            if (evaluationTask != null)
            {
                if (evaluationTask.EvaluationItem.IsGroupal) 
                { 
                    taskIsGroupal= true;
                }
            }
            else 
            {
                return Content("Evaluation Task not found ");
            }

            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", fileName);

                try 
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(stream);
                    }
                } 
                catch(Exception ex) 
                { 
                    Console.WriteLine(ex); 
                }

                if (taskIsGroupal)
                {
                    //Gets the group in which the student is 
                    var group = await _groupRepository.GetGroupWhereStudentIdAsync(model.StudentId, evaluationTask.EvaluationItem.CourseCycleId);
                    if (group != null) 
                    {
                        foreach (var student in group.Students) 
                        {
                            var submission = new TaskSubmission
                            {
                                EvaluationTaskId = model.EvaluationTaskId,
                                StudentId = student.Id,
                                FilePath = Url.Content($"~/uploads/{fileName}"),
                                SubmissionDate = DateTime.Now
                            };

                            // Save the submission to the database
                            var result = await _taskSubmissionRepository.AddAsync(submission);
                            if (!result.Success)
                            {
                                return Content(result.Message);
                            }
                        }
                    }
                }
                else 
                {
                    var submission = new TaskSubmission
                    {
                        EvaluationTaskId = model.EvaluationTaskId,
                        StudentId = model.StudentId,
                        FilePath = Url.Content($"~/uploads/{fileName}"),
                        SubmissionDate = DateTime.Now
                    };

                    // Save the submission to the database
                    var result = await _taskSubmissionRepository.AddAsync(submission);
                    if (!result.Success)
                    {
                        return Content(result.Message);
                    }
                }

                return Content("200");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateScore(int submissionId, decimal? score) 
        {
            try 
            { 
                //Will look for the submission's task and evaluation item to check if it's groupal
                var submission = await _taskSubmissionRepository.GetByIdAsync(submissionId);
                if (submission.EvaluationTask.EvaluationItem.IsGroupal) 
                {
                    //Gets the groups 
                    var groups = await _groupRepository.GetAllByCourseCycleIdAsync(submission.EvaluationTask.EvaluationItem.CourseCycleId);
                    var submissions = await _taskSubmissionRepository.GetAllByEvaluationTaskIdAsync(submission.EvaluationTask.Id);
                    foreach (var group in groups) 
                    { 
                        foreach(var student in group.Students) 
                        {
                            foreach (var tasksubmission in submissions) 
                            {
                                if(tasksubmission.StudentId == student.Id) 
                                {
                                    //Updates the Score of the taskSubmission 
                                    var result = await _taskSubmissionRepository.UpdateScoreAsync(tasksubmission.Id, score);
                                    if (!result.Success)
                                    {
                                        return Content(result.Message);
                                    }
                                }
                            }
                        }
                    }
                } 
                else
                {
                    //Updates the Score of the taskSubmission 
                    var result = await _taskSubmissionRepository.UpdateScoreAsync(submissionId, score);
                    if (!result.Success)
                    {
                        return Content(result.Message);
                    }
                }
                return Content("200");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
