namespace FutreTechAPI.BL
{
    public interface IObjectValidator
    {
        bool Validate(object model);
    }

    public class ObjectValidator : IObjectValidator
    {
        public bool Validate(object model)
        {
            return true;
        }
    }
}
