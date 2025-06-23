using Microsoft.AspNetCore.Mvc;
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
        [ValidateAntiForgeryToken]
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

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("TrainingSystemAPI");
            var response = await client.GetAsync($"Course/GetById/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<GeneralResponse<CourseEditViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(wrapper?.Data);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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


    }
}
