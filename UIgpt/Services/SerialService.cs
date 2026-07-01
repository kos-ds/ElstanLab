using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using ElstanLab.Models;
using ElstanLab.Services;

namespace ElstanLab.Services
{
    public static class SerialService
    {
        //public static event Action<MeterPacket> PacketReceived;

        static string selectedPort;

        static SerialPort port;

        static CancellationTokenSource cts;

        public static bool IsConnected { get; private set; }

        public static LabMode CurrentMode { get; private set; }

        //--------------------------------------------------

        public static event Action<bool> ConnectionChanged;

        public static event Action<LabMode> ModeChanged;

        //--------------------------------------------------

        public static void Start()
        {
            cts = new CancellationTokenSource();

            Task.Run(() => ConnectionLoop(cts.Token));
        }

        //--------------------------------------------------

        static async Task ConnectionLoop(
            CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!IsConnected)
                {
                    TryConnect();
                }

                await Task.Delay(2000);
            }
        }

        //--------------------------------------------------
              
        static void TryConnect()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedPort))
                    return;

                port = new SerialPort();

                port.PortName = selectedPort;

                port.BaudRate = 115200;

                port.ReadTimeout = 1000;

                port.WriteTimeout = 1000;

                port.Open();

                IsConnected = true;

                ConnectionChanged?.Invoke(true);

                //StartReadLoop();
                Task.Run(async () =>
                {
                    await Task.Delay(1000);

                    StartReadLoop();
                });
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Disconnect();
            }
        }
        //--------------------------------------------------
        public static void SetPort(string portName)
        {
            selectedPort = portName;

            Disconnect();
        }

        static void StartReadLoop()
        {
            Task.Run(async () =>
            {
                while (IsConnected)
                {
                    try
                    {
                        port.WriteLine("get");

                        string line = port.ReadLine();

                        MeterPacket packet = PacketParser.Parse(line);

                        if (packet != null)
                        {
                            DataModelService.Update(packet);

                            LabMode newMode = (LabMode)packet.MODE;

                            if (newMode != CurrentMode)
                            {
                                CurrentMode = (LabMode)packet.MODE;

                                ModeChanged?.Invoke(CurrentMode);
                            }
                        }

                        await Task.Delay(500);
                    }
                    catch
                    {
                        Disconnect();
                    }
                }
            });
        }

        //--------------------------------------------------

        static void Parse(string line)
        {
            try
            {
                // MODE=2

                if (line.StartsWith("MODE="))
                {
                    string s =
                        line.Replace("MODE=", "");

                    int mode =
                        Convert.ToInt32(s);

                    CurrentMode =
                        (LabMode)mode;

                    ModeChanged?.Invoke(CurrentMode);
                }
            }
            catch
            {
            }
        }

        //--------------------------------------------------

        static void Disconnect()
        {
            try
            {
                IsConnected = false;

                ConnectionChanged?.Invoke(false);

                port?.Close();
            }
            catch
            {
            }
        }
    }
}