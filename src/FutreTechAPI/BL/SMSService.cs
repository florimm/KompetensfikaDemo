using System.Threading.Tasks;

namespace FutreTechAPI.BL
{
    public interface ISMSService
    {
        Task<bool> Send(string nr);
    }

    public class SMSService : ISMSService
    {
        public Task<bool> Send(string nr)
        {
            return Task.FromResult(true);
        }
    }
}
