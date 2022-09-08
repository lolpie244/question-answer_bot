/// Need for using Dictionary in postsharp aspect
namespace System.Collections.Generic
{
    
    
    [Serializable]
    public class GenericEqualityComparer<T> : EqualityComparer<T>
    {
        public override bool Equals(T? a, T? b)
        {
            return a.Equals(b);
        }

        public override int GetHashCode(T obj)
        {
            return 1;
        }
    }
    [Serializable]
    public class ObjectEqualityComparer<T> : EqualityComparer<T>
    {
        public override bool Equals(T? a, T? b)
        {
            return a.Equals(b);
        }

        public override int GetHashCode(T obj)
        {
            return 1;
        }
    }
    [Serializable]
    public class EnumEqualityComparer<T> : EqualityComparer<T>
    {
        public override bool Equals(T? a, T? b)
        {
            return a.Equals(b);
        }

        public override int GetHashCode(T obj)
        {
            return 1;
        }
    }
}