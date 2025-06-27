using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using TrainingSystemMVC.Models;
using TrainingSystemMVC.ViewModel;

namespace TrainingSystemMVC.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CourseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.GetAsync("Course/GetAll");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<GeneralResponse<List<CourseViewModel>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(wrapper?.Data);
        }
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.GetAsync($"Course/GetById/{id}");
            if (!response.IsSuccessStatusCode)
                return View("Error");
            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<GeneralResponse<CourseViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return View(wrapper?.Data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CourseAddViewModel course)
        {
            if (!ModelState.IsValid)
                return View(course);

            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.PostAsJsonAsync("Course/Add", new CourseAddViewModel
            {
                Title = course.Title,
                InstructorId = course.InstructorId
            });

            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.GetAsync($"Course/GetById/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var wrapper = JsonSerializer.Deserialize<GeneralResponse<CourseEditViewModel>>(json, options);

            //if (wrapper == null || wrapper.Data == null)
            //    return View("Error");

            return View(wrapper.Data);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CourseEditViewModel course)
        {
            if (!ModelState.IsValid)
                return View(course);

            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.PutAsJsonAsync($"Course/Update/{id}", course);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.DeleteAsync($"Course/Delete/{Id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Course deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete course.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
