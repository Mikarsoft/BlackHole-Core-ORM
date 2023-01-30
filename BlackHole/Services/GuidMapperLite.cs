using System.Data;
using Dapper;

namespace BlackHole.Services
{
    public class GuidMapperLite : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            if(value.GetType() == typeof(string))
            {
                return new Guid((string)value);
            }
            return Guid.Empty;
        }
    }
}
