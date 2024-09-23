using System.Text.Json.Serialization;

namespace Mikarsoft.BlackHoleCore.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHStruct<T> : IBHStruct where T : IComparable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Value { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHStruct
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public struct Int : IBHStruct<int>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public readonly Type GetBaseType()
        {
            return typeof(int);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Value => value;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Str : IBHStruct<string>
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
    }

    /// <summary>
    /// 
    /// </summary>
    public struct Uid : IBHStruct<Guid>
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
    }

    public struct Json<T> where T : class
    {
        [JsonPropertyName("value")]
        public T Value { get; set; }

        // Constructor
        public Json(T value)
        {
            Value = value;
        }

        public static implicit operator Json<T>(T value)
        {
            return new Json<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        public static implicit operator T(Json<T> jObject)
        {
            return jObject.Value;
        }
    }
}
