using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CanvasLMS.Controllers
{
    public class AssignmentsController : MainCourseCycleController
    {
        private readonly IEvaluationTaskRepository _evaluationTaskRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEvaluationItemRepository _evaluationItemRepository;
        public AssignmentsController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseCycleRepository courseCycleRepository,
            IEvaluationTaskRepository evaluationTaskRepository, IGroupRepository groupRepository, IEvaluationItemRepository evaluationItemRepository) : base(enrollmentRepository, studentRepository, courseCycleRepository)
        {
            _evaluationTaskRepository = evaluationTaskRepository;
            _groupRepository = groupRepository;
            _evaluationItemRepository = evaluationItemRepository;
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

        [HttpPost]
        public async Task<IActionResult> Task(int id) 
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

            //Gets the task by its id
            var evaluationTask = await _evaluationTaskRepository.GetByIdAsync(id);
            //Converts task into TaskViewModel
            var taskViewModel = new EvaluationTaskViewModel();
            ObjectMapper.MapProperties(evaluationTask, taskViewModel);

            return View(taskViewModel);
        }
    }
}
