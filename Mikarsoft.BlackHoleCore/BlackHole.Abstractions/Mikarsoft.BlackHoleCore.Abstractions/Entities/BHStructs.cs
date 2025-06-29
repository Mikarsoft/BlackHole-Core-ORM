using Mikarsoft.BlackHoleCore.Abstractions;
using System.Text.Json.Serialization;

namespace Mikarsoft.BlackHoleCore.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHStruct
    {
        Type BaseType { get; }
    }

    public struct Int : IBHStruct, IComparable, IComparable<int>, IEquatable<int>
    {
        private const int ByteMask = 0xFF; // Mask to ensure only 8 bits are used

        private int value;

        // Constructor that allows nullable input
        public Int(int? value)
        {
            value = value ?? 0;
        }

        // Implicit conversion from int? to Int
        public static implicit operator Int(int? value) => new Int(value);

        // Implicit conversion from Int to int
        public static implicit operator int(Int number) => number.Value;

        // Implement IComparable<int>
        public int CompareTo(int other)
        {
            return value.CompareTo(other);
        }

        // Implement IEquatable<int>
        public bool Equals(int other)
        {
            return value == other;
        }

        // Override ToString
        public override string ToString()
        {
            return value.ToString();
        }

        // GetBytes (for serializing the Int)
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[4];
            byteArray[0] = (byte)((value >> 24) & ByteMask);
            byteArray[1] = (byte)((value >> 16) & ByteMask);
            byteArray[2] = (byte)((value >> 8) & ByteMask);
            byteArray[3] = (byte)(value & ByteMask);
            return byteArray;
        }

        // Implement IComparable<object> for compatibility with other types
        public int CompareTo(object? obj)
        {
            if (obj is int)
                return CompareTo((int)obj);

            throw new ArgumentException("Object is not an Int32.");
        }

        // Property to retrieve the base value
        public int Value => value;

        // The base type of the struct
        public readonly Type BaseType => typeof(int);

        // Override Equals for object comparison
        public override bool Equals(object? obj)
        {
            if (obj is int otherInt)
                return Equals(otherInt);

            return base.Equals(obj);
        }

        // Hash code for equality comparison
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public struct Str : IBHStruct, IComparable, IComparable<string>, IEquatable<string>
    {
        private string value;

        /// <summary>
        /// 
        /// </summary>
        public string Value => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Str(string? value)
        {
            this.value = value ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Str(string? value)
        {
            return new Str(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static implicit operator string(Str number)
        {
            return number.Value;
        }


        public readonly Type BaseType => typeof(string);

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string? other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(string? other)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Uid : IBHStruct , IComparable, IComparable<Guid>, IEquatable<Guid>
    {
        private Guid value;

        /// <summary>
        /// 
        /// </summary>
        public Guid Value => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Uid(Guid? value)
        {
            this.value = value ?? Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Uid(Guid? value)
        {
            return new Uid(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static implicit operator Guid(Uid number)
        {
            return number.Value;
        }

        public readonly Type BaseType => typeof(Guid);

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Guid other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Guid other)
        {
            throw new NotImplementedException();
        }
    }

    public struct BHJson<T> : IComparable where T : class , new()
    {
        [JsonPropertyName("value")]
        public T Value { get; set; }

        public Type BaseType => typeof(string);

        // Constructor
        public BHJson(T value)
        {
            Value = value;
        }

        public static implicit operator BHJson<T>(T value)
        {
            return new BHJson<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static implicit operator T(BHJson<T> jObject)
        {
            return jObject.Value;
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }
    }

    public struct BHItem<T, G> : IComparable where T : BHEntityAI<T, G>, new() where G : struct, IBHStruct
    {
        private T? Value;

        public BHItem(T? value)
        {
            Value = value;
        }

        public static implicit operator BHItem<T, G>(T? item) => new BHItem<T, G>(item);

        public static implicit operator T?(BHItem<T, G> item) => item.Value;

        internal async Task Include(IMIncludeCaller caller, IBHTransaction transaction, string property, object value)
        {
            Value = await caller.GetItemAsync<T>(property, value, transaction);
        }

        public int CompareTo(object? obj)
        {
            if (obj is BHItem<T, G> item)
            {
                if (item.Value == Value)
                {
                    return 0;
                }
            }

            return -1;
        }
    }

    public struct BHCollection<T, G> : IComparable where T : BHEntityAI<T, G> , new() where G : struct, IBHStruct
    {
        private List<T> Children { get; set; }

        public BHCollection()
        {
            Children = new List<T>();
        }

        public BHCollection(List<T> items)
        {
            Children = items;
        }

        public static implicit operator BHCollection<T, G>(List<T> items) => new BHCollection<T, G>(items);

        public static implicit operator List<T>(BHCollection<T, G> collection) => collection.Children;

        internal async Task Include(IMIncludeCaller caller, IBHTransaction transaction, string property, object value)
        {
            Children = await caller.GetItemsAsync<T>(property, value, transaction);
        }

        public List<T> ToList()
        {
            return Children;
        }

        public int CompareTo(object? obj)
        {
            if (obj is BHCollection<T, G> item)
            {
                if (item.Children == Children)
                {
                    return 0;
                }
            }

            return -1;
        }
    }

    public struct BHResult<T> where T : struct, IComparable<T>
    {
        private T Value;

        public BHResult(T item)
        {
            Value = item;
        }

        public static implicit operator T(BHResult<T> result) => result.Value;
        public static implicit operator BHResult<T>(T item) => new BHResult<T>(item);
    }
}
