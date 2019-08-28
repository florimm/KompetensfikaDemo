using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FutreTechAPI.Common;
using MediatR;
using MediatR.Pipeline;

namespace FutreTechAPI.BL.NotificationV4
{
    public class SendEmail : IRequest<Result>
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class SendEmailValidator : AbstractValidator<SendEmail>
    {
        public SendEmailValidator()
        {
            RuleFor(t=> t.From).Must(t => t.Contains("@"));
            RuleFor(t => t.To).Must(t => t.Contains("@"));
        }
    }

    public class EmailSenderHandler : IRequestHandler<SendEmail, Result>
    {
        private readonly IEmailService emailService;

        public EmailSenderHandler(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public async Task<Result> Handle(SendEmail request, CancellationToken cancellationToken)
        {
            try
            {
                await emailService.Send(request.From, request.To);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }

    public class SendEmailPreProcessing : IRequestPreProcessor<SendEmail>
    {
        public Task Process(SendEmail request, CancellationToken cancellationToken)
        {
            if (request.From == "florim@gmail.com") // example check if is dev and do something else
            {

            }
            return Task.CompletedTask;
        }
    }
}
