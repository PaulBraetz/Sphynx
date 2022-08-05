namespace Sphynx.Middleware
{
    public readonly struct DefaultSphynxOptions : IEquatable<DefaultSphynxOptions>
    {
        public DefaultSphynxOptions(TimeSpan recoveryTime, Int32 capacity)
        {
            RecoveryTime = recoveryTime;
            InitialCapacity = capacity;
        }

        public static readonly DefaultSphynxOptions Empty = default;

        public readonly TimeSpan RecoveryTime;
        public readonly Int32 InitialCapacity;

        public override String ToString()
        {
            return $"{{{nameof(RecoveryTime)}=\"{RecoveryTime:c}\",{nameof(InitialCapacity)}={InitialCapacity}}}";
        }

        public override Boolean Equals(Object? obj)
        {
            return obj is DefaultSphynxOptions options && Equals(options);
        }

        public Boolean Equals(DefaultSphynxOptions other)
        {
            return RecoveryTime.Equals(other.RecoveryTime) &&
                   InitialCapacity == other.InitialCapacity;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(RecoveryTime, InitialCapacity);
        }

        public static Boolean operator ==(DefaultSphynxOptions left, DefaultSphynxOptions right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(DefaultSphynxOptions left, DefaultSphynxOptions right)
        {
            return !(left == right);
        }
    }
}
