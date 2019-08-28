using System.Threading;
using System.Threading.Tasks;
using FutreTechAPI.Common;
using MediatR;

namespace FutreTechAPI.BL.NotificationV4
{
    public class SendSMS : IRequest<Result>
    {
        public string Nr { get; internal set; }
    }

    public class SMSSenderHandler : IRequestHandler<SendSMS, Result>
    {
        private readonly ISMSService smsService;
        public SMSSenderHandler(ISMSService smsService)
        {
            this.smsService = smsService;
        }

        public async Task<Result> Handle(SendSMS request, CancellationToken cancellationToken)
        {
            var result = await smsService.Send(request.Nr);
            if(result)
            {
                return Result.Ok();
            }
            return Result.Fail("Could not send SMS!");
        }
    }
}
