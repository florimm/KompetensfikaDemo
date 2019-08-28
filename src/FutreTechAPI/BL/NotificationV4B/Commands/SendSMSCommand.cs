using FluentValidation;
using FutreTechAPI.Common;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV4B
{
    public class SendSMSCommand : ICommand, ICacheble
    {
        public string Nr { get; set; }

        public (string, string) CacheKeyVal()
        {
            return ("Nr", this.Nr);
        }

        public class SendSMSHandler : ICommandHandler<SendSMSCommand>
        {
            private readonly ISMSService smsService;

            public SendSMSHandler(ISMSService smsService)
            {
                this.smsService = smsService;
            }

            public async Task<Result> Handle(SendSMSCommand src)
            {
                await smsService.Send(src.Nr);
                return Result.Ok();
            }
        }

        public class SendSMSValidation : AbstractValidator<SendSMSCommand>
        {
            public SendSMSValidation()
            {
                RuleFor(t => t.Nr).Must(nr => nr.StartsWith("07"));
            }
        }
    }
}
