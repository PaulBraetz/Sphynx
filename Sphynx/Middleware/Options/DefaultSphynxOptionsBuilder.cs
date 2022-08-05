namespace Sphynx.Middleware
{
	public class DefaultSphynxOptionsBuilder : ISphynxOptionsBuilder<DefaultSphynxOptions>
	{
		public TimeSpan RecoveryTime { get; set; } = TimeSpan.FromSeconds(1);
		public Int32 InitialCapacity { get; set; } = 25;

		public virtual DefaultSphynxOptions Build()
		{
			return new DefaultSphynxOptions(RecoveryTime, InitialCapacity);
		}
	}
}
