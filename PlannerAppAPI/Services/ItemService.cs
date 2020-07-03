using PlannerAppAPI.Data;
using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _db;

        public ItemService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ToDoItem> CreateItemAsync(string planId, string description, DateTime? estimatedDate, string userId)
        {
            var plan = await _db.Plans.FindAsync(planId);

            if (plan == null)
            {
                return null;
            }

            var item = new ToDoItem
            {
                Description = description,
                EstimatedDate = estimatedDate,
                IsDone = false,
                PlanId = planId,
                UserId = userId,
            };

            await _db.ToDoItems.AddAsync(item);
            await _db.SaveChangesAsync();

            return item;
        }

        public async Task<ToDoItem> DeleteItemAsync(string itemId, string userId)
        {
            var item = await _db.ToDoItems.FindAsync(itemId);
            if (item == null || item.UserId != userId)
            {
                return null;
            }

            item.IsDeleted = true;
            item.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return item;
        }

        public async Task<ToDoItem> EditItemAsync(string itemId, string newDescription, DateTime? newEstimatedDate, string userId)
        {
            var item = await _db.ToDoItems.FindAsync(itemId);
            if (item == null || item.UserId != userId || item.IsDone)
            {
                return null;
            }

            item.Description = newDescription;
            item.EstimatedDate = newEstimatedDate;
            item.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return item;
        }

        public IEnumerable<ToDoItem> GetAllItems(string planId, string userId)
        {
            var items = _db.ToDoItems.Where(i => i.PlanId == planId && i.UserId == userId && !i.IsDeleted).ToArray();

            return items;
        }

        public IEnumerable<ToDoItem> GetNotAchievedItems(string userId)
        {
            var items = _db.ToDoItems.Where(i => !i.IsDone && i.UserId == userId && !i.IsDeleted).ToArray();

            return items;
        }

        public async Task<ToDoItem> MarkItemAsync(string itemId, string userId)
        {
            var item = await _db.ToDoItems.FindAsync(itemId);
            if (item == null || item.UserId != userId)
            {
                return null;
            }

            item.IsDone = !item.IsDone;
            item.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return item;
        }
    }
}
