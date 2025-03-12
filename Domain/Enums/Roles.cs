using System.Runtime.Serialization;

namespace StreamsMS.Domain.Enums
{
    public enum Roles
    {
        [EnumMember(Value = "PARTICIPANT")]
        PARTICIPANT,
        [EnumMember(Value = "VIEWER")]
        VIEWER
    }
}
