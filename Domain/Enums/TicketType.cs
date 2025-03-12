using System.Runtime.Serialization;

namespace StreamsMS.Domain.Enums
{
    public enum TicketType
    {
        [EnumMember(Value = "VIEWER")]
        VIEWER,
        [EnumMember(Value = "PARTICIPANT")]
        PARTICIPANT
    }
}
