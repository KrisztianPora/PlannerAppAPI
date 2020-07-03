using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public interface IItemService
    {
        IEnumerable<ToDoItem> GetAllItems(string planId, string userId);
        IEnumerable<ToDoItem> GetNotAchievedItems(string userId);
        Task<ToDoItem> CreateItemAsync(string planId, string description, DateTime? estimatedDate, string userId);
        Task<ToDoItem> EditItemAsync(string itemId, string newDescription, DateTime? newEstimatedDate, string userId);
        Task<ToDoItem> MarkItemAsync(string itemId, string userId);
        Task<ToDoItem> DeleteItemAsync(string itemId, string userId);
    }
}
