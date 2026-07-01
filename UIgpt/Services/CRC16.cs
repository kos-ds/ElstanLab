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
    }
}