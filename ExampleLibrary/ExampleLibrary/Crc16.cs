// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: Provides the 16 bit version of cyclic redundancy check
// NuPkg4Src-Tags: CSharp Source Crc
// NuPkg4Src-Id: ExampleLibrary.Crc16
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-MakePublic: Crc16
// NuPkg4Src-Hash: SHA512Managed:3BB89850740227D30F72113E00C5B527A56FA2245C1A805DA50CC79325030712BC2C04853D6AA3C3FCFCDD24D87563962EC010AD20F8D9F83B2972DD89254778
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace ExampleLibrary
{
    internal static class Crc16
    {
        private const ushort Polynomial = 0xA001;

        private static readonly ushort[] Table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            foreach (byte t in bytes)
            {
                var index = (byte)(crc ^ t);
                crc = (ushort)((crc >> 8) ^ Table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            for (ushort i = 0; i < Table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ Polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                Table[i] = value;
            }
        }
    }
}
