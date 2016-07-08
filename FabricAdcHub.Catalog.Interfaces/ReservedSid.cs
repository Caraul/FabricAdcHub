using FabricAdcHub.Core.Commands;

namespace FabricAdcHub.Catalog.Interfaces
{
    public class ReservedSid
    {
        public ReservedSid(string sid)
        {
            Sid = sid;
            Error = Status.ErrorCode.NoError;
        }

        public ReservedSid(Status.ErrorCode error)
        {
            Error = error;
        }

        public string Sid { get; }

        public Status.ErrorCode Error { get; }
    }
}
