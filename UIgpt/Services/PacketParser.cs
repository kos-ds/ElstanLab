using System;
using ElstanLab.Models;
using UIgpt;
using System.Drawing;


namespace ElstanLab.Services
{
    public static class PacketParser
    {
        public const int PacketSize = 91;
        public static event Action SnapshotRequested;

        public static MeterPacket Parse(byte[] packet)
        {
            if (packet == null)
                return null;

            if (packet.Length != PacketSize)
                return null;

            MeterPacket m = new MeterPacket();

            //----------------------------------------------------
            // Header
            //----------------------------------------------------

            if (packet[0] != 0x55 || packet[1] != 0xAA)
            {
                LabStorage.labsett.status = false;
                MainForm.Instance.SetStatus(" ● Измерители не отвечают", Color.Red);
                return null;
            }

            MainForm.Instance.SetStatus(" ● Данные актуальны", Color.Green);
            //----------------------------------------------------
            // CRC
            //----------------------------------------------------

            ushort crcCalc = CRC16.Compute2(packet, 0, 89);

            ushort crcRecv =
                (ushort)(packet[89] |
                        (packet[90] << 8));

            if (crcCalc != crcRecv)
                return null;

            //----------------------------------------------------

            //MeterPacket m = new MeterPacket();

            int p = 2;

            //----------------------------------------------------
            // Meter 1
            //----------------------------------------------------

            m.U1_A = ReadFloat(packet, ref p);
            m.U1_B = ReadFloat(packet, ref p);
            m.U1_C = ReadFloat(packet, ref p);

            m.UL1_AB = ReadFloat(packet, ref p);
            m.UL1_BC = ReadFloat(packet, ref p);
            m.UL1_CA = ReadFloat(packet, ref p);

            m.I1_A = ReadFloat(packet, ref p);
            m.I1_B = ReadFloat(packet, ref p);
            m.I1_C = ReadFloat(packet, ref p);

            m.P1_A = ReadFloat(packet, ref p) * 1000.0;
            m.P1_B = ReadFloat(packet, ref p) * 1000.0;
            m.P1_C = ReadFloat(packet, ref p) * 1000.0;

            m.PTOTAL = ReadFloat(packet, ref p) * 1000.0;

            m.F1 = ReadFloat(packet, ref p);

            //----------------------------------------------------
            // Meter 2
            //----------------------------------------------------

            m.U2_A = ReadFloat(packet, ref p);
            m.U2_B = ReadFloat(packet, ref p);
            m.U2_C = ReadFloat(packet, ref p);

            m.UL2_AB = ReadFloat(packet, ref p);
            m.UL2_BC = ReadFloat(packet, ref p);
            m.UL2_CA = ReadFloat(packet, ref p);

            m.F2 = ReadFloat(packet, ref p);

            //----------------------------------------------------

            m.MODE = packet[p++];

            m.SNAPSHOT = packet[p++] != 0;

            m.Kct = packet[p++];

            if (m.SNAPSHOT)
            {
                SnapshotRequested?.Invoke();
            }
            //----------------------------------------------------

            return m;
        }

        //----------------------------------------------------

        static float ReadFloat(byte[] data, ref int pos)
        {
            float value = BitConverter.ToSingle(data, pos);

            pos += 4;

            return value;
        }
    }
}