using System.Threading.Tasks;

namespace FutreTechAPI.BL
{
    public interface IEmailService
    {
        Task Send(string email, string to);
    }

    public class EmailService : IEmailService
    {
        public Task Send(string email, string to)
        {
            return Task.CompletedTask;
        }
    }
}
