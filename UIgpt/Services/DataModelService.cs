using System;
using ElstanLab.Models;

namespace ElstanLab.Services
{
    public static class DataModelService
    {
        //------------------------------------------------

        public static MeterPacket CurrentPacket
        {
            get;
            private set;
        }

        //------------------------------------------------

        static bool lastSnapshot;

        //------------------------------------------------

        public static event Action<MeterPacket>
            DataUpdated;

        public static event Action<MeterPacket>
            SnapshotTriggered;

        //------------------------------------------------

        public static void Update(MeterPacket packet)
        {
            CurrentPacket = packet;

            DataUpdated?.Invoke(packet);

            //------------------------------------------------
            // SNAPSHOT FRONT
            //------------------------------------------------

            if (!lastSnapshot && packet.SNAPSHOT)
            {
                SnapshotTriggered?.Invoke(packet);
            }

            lastSnapshot =
                packet.SNAPSHOT;
        }
    }
}