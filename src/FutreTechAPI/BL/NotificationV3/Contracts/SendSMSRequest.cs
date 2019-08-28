using FluentValidation;

namespace FutreTechAPI.BL.NotificationV3.Contracts
{
    public class SendSMSRequest
    {
        public string Nr { get; set; }
    }

    public class SendSMSRequestValidator : AbstractValidator<SendSMSRequest>
    {
        public SendSMSRequestValidator()
        {
            this.RuleFor(t => t.Nr).NotEmpty().Must(t => t.StartsWith("07"));
        }
    }
}
