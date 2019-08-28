using FutreTechAPI.Common;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV4B
{
    public class SendEmailCommand : ICommand
    {
        public string From { get; set; }
        public string To { get; set; }
        public class SendEmailHandler : ICommandHandler<SendEmailCommand>
        {
            private readonly IEmailService emailService;

            public SendEmailHandler(IEmailService emailService)
            {
                this.emailService = emailService;
            }

            public async Task<Result> Handle(SendEmailCommand command)
            {
                await emailService.Send(command.From, command.To);
                return Result.Ok();
            }
        }
    }
}
