using FluentValidation;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV4B.Queries
{
    public class GetCitiyById : IQuery<GetCityByIdResponse>
    {
        public int Id { get; set; }

        public class GetCityByIdHandler : IQueryHandler<GetCitiyById, GetCityByIdResponse>
        {
            public Task<GetCityByIdResponse> Handle(GetCitiyById query)
            {
                var city = new GetCityByIdResponse() { Id = 1, Name = "Norrkoping" };
                return Task.FromResult(city);
            }
        }

        public class GetCityByIdValidator : AbstractValidator<GetCitiyById>
        {
            public GetCityByIdValidator()
            {
                RuleFor(t => t.Id).GreaterThan(0);
            }
        }

    }

    public class GetCityByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
