using System.Text;

namespace ElstanLab.Services
{
    public static class CRC16
    {
        public static ushort Compute(string data)
        {
            ushort crc = 0xFFFF;

            byte[] bytes =
                Encoding.ASCII.GetBytes(data);

            foreach (byte b in bytes)
            {
                crc ^= b;

                for (int i = 0; i < 8; i++)
                {
                    bool lsb =
                        (crc & 0x0001) != 0;

                    crc >>= 1;

                    if (lsb)
                    {
                        crc ^= 0xA001;
                    }
                }
            }

            return crc;
        }

        public static ushort Compute2(byte[] data, int offset, int length)
        {
            ushort crc = 0xFFFF;

            for (int i = offset; i < offset + length; i++)
            {
                crc ^= data[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }

            return crc;
        }


    }
}