using PlannerAppAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public interface IPlanService
    {
        IEnumerable<Plan> GetAllPlans(int pageSize, int pageNumber, string userId, out int totalPlans);
        IEnumerable<Plan> SearchPlans(string query, int pageSize, int pageNumber, string userId, out int totalPlans);
        Task<Plan> AddPlanAsync(string name, string description, string imagePath, string userId);
        Task<Plan> EditPlanAsync(string id, string newName, string newDescription, string newImagePath, string userId);
        Task<Plan> DeletePlanAsync(string id, string userId);
        Task<Plan> GetPlanByIdAsync(string id, string userId);
        Task<Plan> GetPlanByNameAsync(string name, string userId);

    }
}
