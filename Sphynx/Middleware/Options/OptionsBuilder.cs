namespace Sphynx.Middleware
{
    public class OptionsBuilder : IOptionsBuilder
    {
        public TimeSpan RecoveryTime { get; set; } = TimeSpan.FromSeconds(1);
        public Int32 Capacity { get; set; } = 25;

        public Options BuildOptions()
        {
            return new Options(RecoveryTime, Capacity);
        }
    }
}
