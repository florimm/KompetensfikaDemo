namespace FutreTechAPI.BL
{
    public interface ILogger
    {
        void Write(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            System.Diagnostics.Debug.WriteLine($"\n\thttp://{message.Replace(" ","_")}");
        }
    }
}
