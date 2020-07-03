using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlannerAppAPI.Models;
using PlannerAppAPI.Models.Responses;
using PlannerAppAPI.Services;

namespace PlannerAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [ProducesResponseType(200, Type = typeof(CollectionResponse<ToDoItem>))]
        [ProducesResponseType(404)]
        [HttpGet("planId={planId}")]
        public IActionResult Get(string planId)
        {
            if (planId == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var items = _itemService.GetAllItems(planId, userId);

            return Ok(new CollectionResponse<ToDoItem>
            {
                Count = items.Count(),
                IsSuccess = true,
                Message = "Items retrieved successfully",
                OperationDate = DateTime.UtcNow,
                Records = items,
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionResponse<ToDoItem>))]
        [HttpGet("notachieved")]
        public IActionResult Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var items = _itemService.GetNotAchievedItems(userId);

            return Ok(new CollectionResponse<ToDoItem>
            {
                Count = items.Count(),
                IsSuccess = true,
                Message = "Items retrieved successfully",
                OperationDate = DateTime.UtcNow,
                Records = items,
            });
        }

        [ProducesResponseType(200, Type = typeof(CollectionResponse<ToDoItem>))]
        [ProducesResponseType(400, Type = typeof(CollectionResponse<ToDoItem>))]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ToDoItemRequest toDoItemRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var item = await _itemService.CreateItemAsync(toDoItemRequest.PlanId, toDoItemRequest.Description, toDoItemRequest.EstimatedDate, userId);

                return Ok(new OperationResponse<ToDoItem>
                {
                    IsSuccess = true,
                    Message = "Item inserted successfully",
                    OperationDate = DateTime.UtcNow,
                    Record = item,
                });
            }

            return BadRequest(new OperationResponse<ToDoItem>
            {
                IsSuccess = false,
                Message = "Item insert failed",
            });
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<ToDoItem>))]
        [ProducesResponseType(400, Type = typeof(OperationResponse<ToDoItem>))]
        [ProducesResponseType(404)]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ToDoItemRequest toDoItemRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var item = await _itemService.EditItemAsync(toDoItemRequest.Id, toDoItemRequest.Description, toDoItemRequest.EstimatedDate, userId);
                if (item == null)
                {
                    return NotFound();
                }

                return Ok(new OperationResponse<ToDoItem>
                {
                    IsSuccess = true,
                    Message = "Item edited successfully",
                    OperationDate = DateTime.UtcNow,
                    Record = item,
                });
            }

            return BadRequest(new OperationResponse<ToDoItem>
            {
                IsSuccess = false,
                Message = "Item edit failed",
            });
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<ToDoItem>))]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var item = await _itemService.MarkItemAsync(id, userId);
            if (item == null)
                return NotFound();

            return Ok(new OperationResponse<ToDoItem>
            {
                IsSuccess = true,
                Message = "Item status changed successfully",
                OperationDate = DateTime.UtcNow,
                Record = item
            });
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<ToDoItem>))]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var item = await _itemService.DeleteItemAsync(id, userId);
            if (item == null)
                return NotFound();

            return Ok(new OperationResponse<ToDoItem>
            {
                IsSuccess = true,
                Message = "Item deleted changed successfully",
                OperationDate = DateTime.UtcNow,
                Record = item
            });
        }
    }
}
