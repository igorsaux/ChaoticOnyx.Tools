#region

using System;

#pragma warning disable 8618

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public sealed class Change : IEquatable<Change>
    {
        public string Prefix
        {
            get;
            init;
        }

        public string Message
        {
            get;
            init;
        }

        public Change()
        {
            Prefix  = string.Empty;
            Message = string.Empty;
        }
        
        public static bool operator ==(Change a, Change b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Change a, Change b)
        {
            return !a.Equals(b);
        }

        public bool Equals(Change? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Prefix == other.Prefix && Message == other.Message;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Change) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Prefix, Message);
        }
    }
}
