using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PlannerAppAPI.Models;
using PlannerAppAPI.Models.Responses;
using PlannerAppAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlannerAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;
        private readonly IConfiguration _configuration;
        private const int PAGE_SIZE = 10;

        public PlansController(IPlanService planService, IConfiguration configuration)
        {
            _planService = planService;
            _configuration = configuration;
        }

        private readonly List<string> allowedExtensions = new List<string>
        {
            ".jpg", ".bmp", ".png"
        };

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Plan>))]
        [HttpGet]
        public IActionResult Get(int page)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalPlans = 0;
            if (page == 0)
            {
                page = 1;
            }
            var plans = _planService.GetAllPlans(PAGE_SIZE, page, userId, out totalPlans);

            int totalPages = 0;
            if (totalPlans % PAGE_SIZE == 0)
            {
                totalPages = totalPlans / PAGE_SIZE;
            }
            else
            {
                totalPages = (totalPlans / PAGE_SIZE) + 1;
            }

            return Ok(new CollectionPagingResponse<Plan>
            {
                Count = totalPlans,
                IsSuccess = true,
                Message = "Plans received successfully",
                OperationDate = DateTime.UtcNow,
                Page = page,
                PageSize = PAGE_SIZE,
                Records = plans,
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Plan>))]
        [HttpGet("search")]
        public IActionResult Get(string query, int page)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int totalPlans = 0;
            if (page == 0)
            {
                page = 1;
            }
            var plans = _planService.SearchPlans(query, PAGE_SIZE, page, userId, out totalPlans);

            int totalPages = 0;
            if (totalPlans % PAGE_SIZE == 0)
            {
                totalPages = totalPlans / PAGE_SIZE;
            }
            else
            {
                totalPages = (totalPlans / PAGE_SIZE) + 1;
            }

            return Ok(new CollectionPagingResponse<Plan>
            {
                Count = totalPlans,
                IsSuccess = true,
                Message = $"Plans of '{query}' received successfully",
                OperationDate = DateTime.UtcNow,
                Page = page,
                PageSize = PAGE_SIZE,
                Records = plans,
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Plan>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Plan>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var plan = await _planService.GetPlanByIdAsync(id, userId);
            if (plan == null)
            {
                return BadRequest(new OperationResponse<string>
                {
                    IsSuccess = false,
                    Message = "Invalid operation"
                });
            }

            return Ok(new OperationResponse<Plan>
            {
                IsSuccess = true,
                Message = "Plan received successfully",
                OperationDate = DateTime.UtcNow,
                Record = plan,
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Plan>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Plan>))]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]PlanRequest plan)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/DefaultPlan.jpg";
            string fullPath = null;
            if (plan.CoverFile != null)
            {
                string extension = Path.GetExtension(plan.CoverFile.FileName);

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new OperationResponse<string>
                    {
                        IsSuccess = false,
                        Message = "Image type not supported",
                    });
                }

                if (plan.CoverFile.Length > 500000)
                {
                    return BadRequest(new OperationResponse<string>
                    {
                        IsSuccess = false,
                        Message = "Image size cannot be bigger than 5MB",
                    });
                }

                string newFileName = $"Images/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }

            var addedPlan = await _planService.AddPlanAsync(plan.Title, plan.Description, url, userId);

            if (addedPlan != null)
            {
                if (fullPath != null)
                {
                    using(var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await plan.CoverFile.CopyToAsync(fileStream);
                    }
                }

                return Ok(new OperationResponse<Plan>
                {
                    IsSuccess = true,
                    Message = $"{addedPlan.Title} has been added successfully",
                    OperationDate = DateTime.UtcNow,
                    Record = addedPlan,
                });
            }

            return BadRequest(new OperationResponse<string>
            {
                IsSuccess = false,
                Message = "Adding plan failed",
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionPagingResponse<Plan>))]
        [ProducesResponseType(400, Type = typeof(CollectionPagingResponse<Plan>))]
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] PlanRequest plan)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string url = $"{_configuration["AppUrl"]}Images/DefaultPlan.jpg";
            string fullPath = null;
            if (plan.CoverFile != null)
            {
                string extension = Path.GetExtension(plan.CoverFile.FileName);

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new OperationResponse<string>
                    {
                        IsSuccess = false,
                        Message = "Image type not supported",
                    });
                }

                if (plan.CoverFile.Length > 500000)
                {
                    return BadRequest(new OperationResponse<string>
                    {
                        IsSuccess = false,
                        Message = "Image size cannot be bigger than 5MB",
                    });
                }

                string newFileName = $"Images/{Guid.NewGuid()}{extension}";
                fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newFileName);
                url = $"{_configuration["AppUrl"]}{newFileName}";
            }

            var oldPlan = await _planService.GetPlanByIdAsync(plan.Id, userId);
            if (fullPath == null)
            {
                url = oldPlan.CoverPath;
            }

            var editedPlan = await _planService.EditPlanAsync(plan.Id, plan.Title, plan.Description, url, userId);

            if (editedPlan != null)
            {
                if (fullPath != null)
                {
                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                    {
                        await plan.CoverFile.CopyToAsync(fileStream);
                    }
                }

                return Ok(new OperationResponse<Plan>
                {
                    IsSuccess = true,
                    Message = $"{editedPlan.Title} has been added successfully",
                    OperationDate = DateTime.UtcNow,
                    Record = editedPlan,
                });
            }

            return BadRequest(new OperationResponse<string>
            {
                IsSuccess = false,
                Message = "Adding plan failed",
            });
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<Plan>))]
        [ProducesResponseType(404)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var oldPlan = await _planService.GetPlanByIdAsync(id, userId);
            if (oldPlan == null)
            {
                return NotFound();
            }

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldPlan.CoverPath.Replace(_configuration["AppUrl"], ""));
            System.IO.File.Delete(fullPath);

            var deletedPlan = await _planService.DeletePlanAsync(id, userId);

            return Ok(new OperationResponse<Plan>
            {
                IsSuccess = true,
                Message = $"{oldPlan.Title} has been deleted successfully",
                OperationDate = DateTime.UtcNow,
                Record = oldPlan,
            });
        }
    }
}
