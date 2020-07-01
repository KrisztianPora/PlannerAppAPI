using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string address, string subject, string content);
    }
}
