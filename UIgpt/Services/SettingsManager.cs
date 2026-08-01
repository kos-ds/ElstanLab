using System;
using System.IO;
using ElstanLab.Models;

namespace ElstanLab.Services
{
    public static class SettingsManager
    {
        private static readonly string Folder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                "ElstanLab");

        private static readonly string FileName =
            Path.Combine(Folder, "settings2.dat");

        //////////////////////////////////////////////////
        // SAVE
        //////////////////////////////////////////////////

        public static void Save()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            using (BinaryWriter bw =
                new BinaryWriter(File.Open(FileName, FileMode.Create)))
            {
                bw.Write(LabStorage.labsett.NoLoadDeltaU);
                bw.Write(LabStorage.labsett.NoLoadDeltaI);
                bw.Write(LabStorage.labsett.NoLoadP0Deviation);
                bw.Write(LabStorage.labsett.NoLoadI0Deviation);

                bw.Write(LabStorage.labsett.ShortCircuitUkDeviation);
                bw.Write(LabStorage.labsett.ShortCircuitPkDeviation);
                bw.Write(LabStorage.labsett.ShortCircuitVoltageDelta);
                bw.Write(LabStorage.labsett.ShortCircuitCurrentDelta);

                bw.Write(LabStorage.labsett.RatioDeviation);
                bw.Write(LabStorage.labsett.RatioKdeviation);
                bw.Write(LabStorage.labsett.IVWTime);
                bw.Write(LabStorage.labsett.AVTime);
                bw.Write(LabStorage.labsett.IVWDeviation);
                bw.Write(LabStorage.labsett.AVDeviation);

                bw.Write(LabStorage.labsett.AutoSelectSnapshot);
            }
        }

        //////////////////////////////////////////////////
        // LOAD
        //////////////////////////////////////////////////

        public static void Load()
        {
            if (!File.Exists(FileName))
                return;

            using (BinaryReader br = new BinaryReader(File.OpenRead(FileName)))
            {
                LabStorage.labsett.NoLoadDeltaU = br.ReadDouble();

                LabStorage.labsett.NoLoadDeltaI = br.ReadDouble();

                LabStorage.labsett.NoLoadP0Deviation = br.ReadDouble();

                LabStorage.labsett.NoLoadI0Deviation = br.ReadDouble();

                LabStorage.labsett.ShortCircuitUkDeviation = br.ReadDouble();

                LabStorage.labsett.ShortCircuitPkDeviation = br.ReadDouble();

                LabStorage.labsett.ShortCircuitVoltageDelta = br.ReadDouble();

                LabStorage.labsett.ShortCircuitCurrentDelta = br.ReadDouble();

                LabStorage.labsett.RatioDeviation = br.ReadDouble();

                LabStorage.labsett.RatioKdeviation = br.ReadDouble();
                
                LabStorage.labsett.IVWTime = br.ReadDouble();
                
                LabStorage.labsett.AVTime = br.ReadDouble();

                LabStorage.labsett.IVWDeviation = br.ReadDouble();

                LabStorage.labsett.AVDeviation = br.ReadDouble();

                LabStorage.labsett.AutoSelectSnapshot = br.ReadBoolean();
            }
        }
    }
}