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

    /// <summary>
    /// 
    /// </summary>
    public struct Int : IBHStruct, IComparable
    {
        private const int ByteMask = 0xFF; // Mask to ensure only 8 bits are used

        private int value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Int(int? value)
        {
            this.value = value ?? 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Int(int? value)
        {
            return new Int(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static implicit operator int(Int number)
        {
            return number.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            // We need 4 bytes for an Int32
            byte[] byteArray = new byte[4];

            // Extract the 4 bytes from the Int32 value
            byteArray[0] = (byte)((value >> 24) & ByteMask);  // Most significant byte
            byteArray[1] = (byte)((value >> 16) & ByteMask);  // Second most significant byte
            byteArray[2] = (byte)((value >> 8) & ByteMask);   // Third most significant byte
            byteArray[3] = (byte)(value & ByteMask);          // Least significant byte

            return byteArray;
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly Type BaseType => typeof(int);

        /// <summary>
        /// 
        /// </summary>
        public int Value => value;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Str : IBHStruct, IComparable
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
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Uid : IBHStruct
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
    }

    public struct BHJson<T> : IBHStruct, IComparable where T : class
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

    public struct BHCollection<T> where T : BHEntity<T>
    {
        private List<T> Children;

        public BHCollection()
        {
            Children = new List<T>();
        }

        public BHCollection(List<T> items)
        {
            Children = items;
        }

        public static implicit operator BHCollection<T>(List<T> items) => new BHCollection<T>(items);

        public static implicit operator List<T>(BHCollection<T> collection) => collection.Children;
    }
}
