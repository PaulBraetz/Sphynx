namespace Sphynx.Middleware
{
    public readonly struct Options : IEquatable<Options>
    {
        public Options(TimeSpan recoveryTime, Int32 capacity)
        {
            RecoveryTime = recoveryTime;
            Capacity = capacity;
        }

        public static readonly Options Empty = default;

        public readonly TimeSpan RecoveryTime;
        public readonly Int32 Capacity;

        public override String ToString()
        {
            return $"{{{nameof(RecoveryTime)}=\"{RecoveryTime:c}\",{nameof(Capacity)}={Capacity}}}";
        }

        public override Boolean Equals(Object? obj)
        {
            return obj is Options options && Equals(options);
        }

        public Boolean Equals(Options other)
        {
            return RecoveryTime.Equals(other.RecoveryTime) &&
                   Capacity == other.Capacity;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(RecoveryTime, Capacity);
        }

        public static Boolean operator ==(Options left, Options right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(Options left, Options right)
        {
            return !(left == right);
        }
    }
}
