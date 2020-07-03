using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlannerAppAPI.Data;
using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public class PlanService : IPlanService
    {
        private readonly ApplicationDbContext _db;

        public PlanService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Plan> AddPlanAsync(string name, string description, string imagePath, string userId)
        {
            var plan = new Plan
            {
                Title = name,
                Description = description,
                CoverPath = imagePath,
                UserId = userId,
            };

            await _db.Plans.AddAsync(plan);
            await _db.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> DeletePlanAsync(string id, string userId)
        {
            var plan = await _db.Plans.FindAsync(id);

            if (plan.UserId != userId || plan.IsDeleted)
            {
                return null;
            }

            plan.IsDeleted = true;
            plan.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return plan;
        }

        public async Task<Plan> EditPlanAsync(string id, string newName, string newDescription, string newImagePath, string userId)
        {
            var plan = await _db.Plans.FindAsync(id);

            if (plan.UserId != userId || plan.IsDeleted)
            {
                return null;
            }

            plan.Title = newName;
            plan.Description = newDescription;
            if (newImagePath != null)
            {
                plan.CoverPath = newImagePath;
            }
            plan.ModifiedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return plan;
        }

        public IEnumerable<Plan> GetAllPlans(int pageSize, int pageNumber, string userId, out int totalPlans)
        {
            var allPlans = _db.Plans.Where(p => !p.IsDeleted && p.UserId == userId);
            totalPlans = allPlans.Count();

            var plans = allPlans.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            foreach (var plan in plans)
            {
                plan.ToDoItems = _db.ToDoItems.Where(i => !i.IsDeleted && i.PlanId == plan.Id).ToArray();
            }

            return plans;
        }

        public async Task<Plan> GetPlanByIdAsync(string id, string userId)
        {
            var plan = await _db.Plans.FindAsync(id);

            if (plan.UserId != userId || plan.IsDeleted)
            {
                return null;
            }

            plan.ToDoItems = _db.ToDoItems.Where(i => !i.IsDeleted && i.PlanId == plan.Id && i.UserId == userId).ToArray();

            return plan;
        }

        public async Task<Plan> GetPlanByNameAsync(string name, string userId)
        {
            var plan = await _db.Plans.SingleOrDefaultAsync(p => p.Title == name && p.UserId == userId);

            if (plan.UserId != userId || plan.IsDeleted)
            {
                return null;
            }

            plan.ToDoItems = _db.ToDoItems.Where(i => !i.IsDeleted && i.PlanId == plan.Id && i.UserId == userId).ToArray();

            return plan;
        }

        public IEnumerable<Plan> SearchPlans(string query, int pageSize, int pageNumber, string userId, out int totalPlans)
        {
            var allPlans = _db.Plans.Where(p => !p.IsDeleted && p.UserId == userId && (p.Description.Contains(query) || p.Title.Contains(query)));
            totalPlans = allPlans.Count();

            var plans = allPlans.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            foreach (var plan in plans)
            {
                plan.ToDoItems = _db.ToDoItems.Where(i => !i.IsDeleted && i.PlanId == plan.Id).ToArray();
            }

            return plans;
        }
    }
}
