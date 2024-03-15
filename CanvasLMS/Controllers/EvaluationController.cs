using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;
using CanvasLMS.Models.ViewModels;
using CanvasLMS.Repositories;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CanvasLMS.Controllers
{
    public class EvaluationController : MainCourseCycleController
    {
        private readonly IEvaluationItemRepository _evaluationItemRepository;
        private readonly IGroupRepository _groupRepository;

        public EvaluationController(IEnrollmentRepository enrollmentRepository, IStudentRepository studentRepository, ICourseCycleRepository courseCycleRepository, 
            IEvaluationItemRepository evaluationItemRepository, IGroupRepository groupRepository) : base(enrollmentRepository, studentRepository, courseCycleRepository)
        { 
            _evaluationItemRepository = evaluationItemRepository;
            _groupRepository = groupRepository;
        }

        public async Task<IActionResult> Items(int id)//Refers to the CourseCycle Id
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

            //Get the groups per CourseCycle to be passed to the View as ViewData
            var groups = await _groupRepository.GetAllByCourseCycleIdAsync(id);
            var groupsViewModel = new List<GroupViewModel>();
            ObjectMapper.MapProperties(groups, groupsViewModel);
            ViewData["Groups"] = groups;

            //Gets the evaluationItems for the courseCycleId
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
        public async Task<IActionResult> CreateItem(EvaluationItemViewModel evaluationItem)
        {
            if (ModelState.IsValid)
            {
                var evaluationDTO = new EvaluationItem();
                ObjectMapper.MapProperties(evaluationItem, evaluationDTO);
                (bool Success, string Message) addItem = await _evaluationItemRepository.AddAsync(evaluationDTO);

                return Content(addItem.Message);
            }
            return Content("There was an issue adding the evaluation item.");
        }

        //Method to Update an Evaluation Item
        [HttpPost]
        public async Task<IActionResult> UpdateItem(EvaluationItemViewModel evaluationItem)
        {
            if (ModelState.IsValid)
            {
                var evaluationDTO = new EvaluationItem();
                ObjectMapper.MapProperties(evaluationItem, evaluationDTO);
                (bool Success, string Message) updateItem = await _evaluationItemRepository.UpdateAsync(evaluationDTO);

                return Content(updateItem.Message);
            }
            return Content("There was an issue adding the evaluation item.");
        }


        //Method to Delete an Evaluatoin
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int evaluationItemId)
        {
            var evaluationItem = await _evaluationItemRepository.GetByIdAsync(evaluationItemId);
            if (evaluationItem != null)
            {
                (bool Success, string Message) remove = await _evaluationItemRepository.DeleteAsync(evaluationItemId);
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


    }
}
