using System;
using ElstanLab.Models;

namespace ElstanLab.Services
{
    public static class PacketParser
    {
        public static MeterPacket Parse(
            string packet)
        {
            try
            {
                //------------------------------------------------
                // Проверка $
                //------------------------------------------------

                if (!packet.StartsWith("$"))
                    return null;

                //------------------------------------------------
                // Удаляем $
                //------------------------------------------------

                packet = packet.Substring(1);

                //------------------------------------------------
                // Ищем CRC
                //------------------------------------------------

                int last =
                    packet.LastIndexOf(';');

                if (last <= 0)
                    return null;

                //------------------------------------------------
                // DATA
                //------------------------------------------------

                string data = packet.Substring(0, last+1);

                //------------------------------------------------
                // CRC HEX
                //------------------------------------------------

                string crcHex = packet.Substring(last + 1).Trim();

                //------------------------------------------------
                // CRC CALC
                //------------------------------------------------

                ushort calc = CRC16.Compute(data);

                ushort recv = Convert.ToUInt16(crcHex, 16);

                //------------------------------------------------
                // CRC ERROR
                //------------------------------------------------

                if (calc != recv)
                    return null;

                //------------------------------------------------
                // SPLIT
                //------------------------------------------------

                string[] p =
                    data.Split(';');

                if (p.Length < 25)
                    return null;

                //------------------------------------------------

                MeterPacket m = new MeterPacket();

                //------------------------------------------------
                // Прибор 1
                //------------------------------------------------

                m.U1_A = ToDouble(p[0]);
                m.U1_B = ToDouble(p[1]);
                m.U1_C = ToDouble(p[2]);

                m.UL1_AB = ToDouble(p[3]);
                m.UL1_BC = ToDouble(p[4]);
                m.UL1_CA = ToDouble(p[5]);

                m.I1_A = ToDouble(p[6]);
                m.I1_B = ToDouble(p[7]);
                m.I1_C = ToDouble(p[8]);

                m.P1_A = ToDouble(p[9]);
                m.P1_B = ToDouble(p[10]);
                m.P1_C = ToDouble(p[11]);

                m.PTOTAL = ToDouble(p[12]);

                m.F1 = ToDouble(p[13]);

                //------------------------------------------------
                // Прибор 2
                //------------------------------------------------

                m.U2_A = ToDouble(p[14]);
                m.U2_B = ToDouble(p[15]);
                m.U2_C = ToDouble(p[16]);

                m.UL2_AB = ToDouble(p[17]);
                m.UL2_BC = ToDouble(p[18]);
                m.UL2_CA = ToDouble(p[19]);

                m.F2 = ToDouble(p[20]);

                //------------------------------------------------
                // COMM
                //------------------------------------------------

                m.COMM1 =
                    p[21] == "1";

                m.COMM2 =
                    p[22] == "1";

                //------------------------------------------------
                // MODE
                //------------------------------------------------

                m.MODE =
                    Convert.ToInt32(p[23]);

                //------------------------------------------------
                // SNAPSHOT
                //------------------------------------------------

                m.SNAPSHOT = p[24] == "1";

                //------------------------------------------------

                return m;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        //----------------------------------------------------

        static double ToDouble(string s)
        {
            return Convert.ToDouble(s) / 1000.0;
        }
    }
}