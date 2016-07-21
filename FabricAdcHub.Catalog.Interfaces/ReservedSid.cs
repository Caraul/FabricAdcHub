using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.Catalog.Interfaces
{
    public class ReservedSid
    {
        public string Sid { get; set; }

        public Status.ErrorCode Error { get; set; }

        public static ReservedSid CreateAsSid(string sid)
        {
            return new ReservedSid
            {
                Sid = sid,
                Error = Status.ErrorCode.NoError
            };
        }

        public static ReservedSid CreateAsError(Status.ErrorCode error)
        {
            return new ReservedSid
            {
                Error = error
            };
        }
    }
}
