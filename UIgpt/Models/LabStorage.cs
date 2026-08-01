using DocumentFormat.OpenXml.Office2013.WebExtension;
using System;
using System.Collections.Generic;

namespace ElstanLab.Models
{
    public class CurrentTransformer
    {
        public int Divider;      // 5,10,20,50
        public string Command;   // g1,g2,g4,g10
    }

    public static class LabStorage
    {
        // ПАСПОРТ
        public static PassportModel Passport = new PassportModel();

        // ХХ
        public static NoLoadSnapshot CurrentNoLoad = new NoLoadSnapshot();
        public static List<NoLoadSnapshot> NoLoadSnapshots = new List<NoLoadSnapshot>();


        // КТР
         public static RatioRealtimeData CurrentKtr = new RatioRealtimeData();

         public static List<RatioRealtimeData> KtrSnapshots = new List<RatioRealtimeData>();


        // КЗ
         public static ShortCircuitSnapshot CurrentKz = new ShortCircuitSnapshot();
         public static List<ShortCircuitSnapshot> KzSnapshots = new List<ShortCircuitSnapshot>();

        // Setting
        public static LabSettings labsett = new LabSettings();

        // IVW (проверка стабильности U/I)
        public static IVWSnapshot CurrentIVW = new IVWSnapshot();
        public static List<IVWSnapshot> IVWSnapshots = new List<IVWSnapshot>();

        // AV (проверка стабильности U/I)
        public static AVSnapshot CurrentAV = new AVSnapshot();
        public static List<AVSnapshot> AVSnapshots = new List<AVSnapshot>();

        public static List<AVSnapshot> AVSnapshotsForReport { get; set; } = new List<AVSnapshot>();

    }

    public class IVWSnapshot
    {
        //------------------------------------------------
        // Time
        //------------------------------------------------
        public DateTime Time;

        //------------------------------------------------
        // Средние значения
        //------------------------------------------------
        public double UaMean;
        public double UbMean;
        public double UcMean;
        public double Uavg;

        public double IaMean;
        public double IbMean;
        public double IcMean;
        public double Iavg;

        //------------------------------------------------
        // Максимальные относительные отклонения (%)
        //------------------------------------------------
        public double UaDev;
        public double UbDev;
        public double UcDev;
        public double MaxUDev;

        public double IaDev;
        public double IbDev;
        public double IcDev;
        public double MaxIDev;

        //------------------------------------------------
        // Результат
        //------------------------------------------------
        public bool Passed;
        public int rowcheckid = 0;

        //------------------------------------------------
        // Полные данные записи (для графика в отчёте)
        //------------------------------------------------
        public List<double> Times = new List<double>();
        public List<double> Ua = new List<double>();
        public List<double> Ub = new List<double>();
        public List<double> Uc = new List<double>();
        public List<double> Ia = new List<double>();
        public List<double> Ib = new List<double>();
        public List<double> Ic = new List<double>();
    }

    public class AVSnapshot
    {
        public DateTime Time { get; set; }
        public string Winding { get; set; } = "ВН";   // "ВН" или "НН"
        public double RequiredU { get; set; }          // норма, кВ

        public List<double> Times { get; set; } = new List<double>();
        public List<double> Ua { get; set; } = new List<double>(); // уже в кВ
        public List<double> Ub { get; set; } = new List<double>();
        public List<double> Uc { get; set; } = new List<double>();
        public List<double> Ia { get; set; } = new List<double>();
        public List<double> Ib { get; set; } = new List<double>();
        public List<double> Ic { get; set; } = new List<double>();

        public double UaMean, Uavg;
        public double IaMean, Iavg;
        public double UaDev, MaxUDev;
        public double IaDev, MaxIDev;

        public bool Passed { get; set; }
    }

    /*   public class AVSnapshot
       {
           //------------------------------------------------
           // Time
           //------------------------------------------------
           public DateTime Time;

           //------------------------------------------------
           // Средние значения
           //------------------------------------------------
           public double UaMean;
           public double UbMean;
           public double UcMean;
           public double Uavg;

           public double IaMean;
           public double IbMean;
           public double IcMean;
           public double Iavg;

           //------------------------------------------------
           // Максимальные относительные отклонения (%)
           //------------------------------------------------
           public double UaDev;
           public double UbDev;
           public double UcDev;
           public double MaxUDev;

           public double IaDev;
           public double IbDev;
           public double IcDev;
           public double MaxIDev;

           //------------------------------------------------
           // Результат
           //------------------------------------------------
           public bool Passed;
           public int rowcheckid = 0;

           //------------------------------------------------
           // Полные данные записи (для графика в отчёте)
           //------------------------------------------------
           public List<double> Times = new List<double>();
           public List<double> Ua = new List<double>();
           public List<double> Ub = new List<double>();
           public List<double> Uc = new List<double>();
           public List<double> Ia = new List<double>();
           public List<double> Ib = new List<double>();
           public List<double> Ic = new List<double>();
       }
    */
    public class RatioRealtimeData
    {
        public double HvAB;
        public double HvBC;
        public double HvCA;
        public double HvAVG;

        public double LvAB;
        public double LvBC;
        public double LvCA;
        public double LvAVG;

        public double KAB;
        public double KBC;
        public double KCA;

        public double KAVG;

        public double kNominal;

        public double Dev;
        public double Err;

        public DateTime Time;

        public bool Passed;

        public int rowcheckid;
        public double HvPercent;
        public double LvPercent;
        
    }

    public class PassportModel
    {
        // Основные сведения

        public string Customer;

        public string ObjectName;

        public DateTime TestDate = DateTime.Now;

        public string Engineer;

        public string Note;


        // Паспорт

        public string Factory = "Elstan";

        public int Year = 2026;

        public string Type;

        public string Serial;

        public double PowerKva = 160;

        public double Frequency = 50;

        public string VectorGroup = "Y/Y-0";

        public string Cooling = "ONAN";


        // ВН

        public double HVVoltage = 10;

        public int HVTapCount = 1;

        public double HVPercent;


        // НН

        public double LVVoltage = 0.4;

        public int LVTapCount = 1;

        public double LVPercent;


        // Расчетные

        public double Ratio;

        public double IHV;

        public double ILV;


        // Паспортные данные

        public double UkPercent = 4.5;

        public double PkLoss = 5000;

        public double P0Loss = 300;

        public double I0Percent = 2;
    }

    public class NoLoadSnapshot
    {
        //------------------------------------------------
        // Time
        //------------------------------------------------

        public DateTime Time;

        //------------------------------------------------
        // Voltages
        //------------------------------------------------

        public double Uab;
        public double Ubc;
        public double Uca;
        public double Uavg;
        public double deltaU;

        //------------------------------------------------
        // Currents
        //------------------------------------------------

        public double Ia;
        public double Ib;
        public double Ic;
        public double Iavg;
        public double deltaI;

        //------------------------------------------------
        // Power
        //------------------------------------------------

        public double Pa;
        public double Pb;
        public double Pc;
        public double Ptotal;

        //------------------------------------------------
        // Cos
        //------------------------------------------------

        public double CosPhi;
        public double Inom;

        public double I0;
        public double P0;
        public double I0Passp;
        public double P0Passp;
        public double I0Otklon;
        public double P0Otklon;

        //------------------------------------------------
        // IEC
        //------------------------------------------------

        public bool Passed;

        public int rowcheckid = 0;
    }

    public class LabSettings
    {
        //////////////////////////////////////////////////
        // Холостой ход
        //////////////////////////////////////////////////

        public double NoLoadDeltaU = 2.0;
        public double NoLoadDeltaI = 2.0;
        public double NoLoadP0Deviation = 10.0;
        public double NoLoadI0Deviation = 10.0;

        //////////////////////////////////////////////////
        // Короткое замыкание
        //////////////////////////////////////////////////

        public double ShortCircuitUkDeviation = 10.0;      // Отклонение Uk от паспорта, %
        public double ShortCircuitPkDeviation = 10.0;      // Отклонение Pk от паспорта, %

        public double ShortCircuitVoltageDelta = 2.0;      // ΔU между фазами, %
        public double ShortCircuitCurrentDelta = 5.0;      // ΔI между фазами, %

        //////////////////////////////////////////////////
        // Коэффициент трансформации
        //////////////////////////////////////////////////

        public double RatioDeviation = 0.5;
        public double RatioKdeviation = 0.5;

        //////////////////////////////////////////////////
        // Общие
        //////////////////////////////////////////////////
        public double IVWTime = 60.0;
        public double IVWDeviation = 20.0;
        public double AVTime = 60.0;
        public double AVDeviation = 20.0;

        public bool AutoSelectSnapshot = true;
        public bool status;
        public bool connect;

        public string sendData = "c";

        public bool check;
    }

    public class ShortCircuitSnapshot
    {
        //------------------------------------------------
        // Time
        //------------------------------------------------

        public DateTime Time;

        //------------------------------------------------
        // Voltages
        //------------------------------------------------

        public double Uab;
        public double Ubc;
        public double Uca;
        public double Uavg;
        public double deltaU;

        //------------------------------------------------
        // Currents
        //------------------------------------------------

        public double Ia;
        public double Ib;
        public double Ic;
        public double Iavg;
        public double deltaI;

        //------------------------------------------------
        // Power
        //------------------------------------------------

        public double Pa;
        public double Pb;
        public double Pc;
        public double Ptotal;

        //------------------------------------------------
        // Calculated
        //------------------------------------------------

        public double UkPercent;

        public double Zk;

        public double Rk;

        public double Xk;

        //------------------------------------------------
        // Expected
        //------------------------------------------------

        public double NominalCurrent;

        public double ExpectedUkVoltage;

        //------------------------------------------------
        // IEC
        //------------------------------------------------

        public bool Passed;

        //////////////////////////////////////////////////
        // Test mode
        //////////////////////////////////////////////////

        public bool Recalculated;

        public double CurrentPercent;

        //////////////////////////////////////////////////
        // Corrected
        //////////////////////////////////////////////////

        public double CorrectedUkPercent;

        public double CorrectedLosses;

        public double UkPassp;
        public double PkPassp;
        public double UkOtklon;
        public double PkOtklon;

        public int rowcheckid = 0;
    }
}




