using Albireo.Base32;

namespace FabricAdcHub.Core.Utilites
{
    public static class AdcBase32Encoder
    {
        public static string Encode(byte[] data)
        {
            return Base32.Encode(data).TrimEnd('=');
        }

        public static byte[] Decode(string data)
        {
            return Base32.Decode(data);
        }
    }
}
