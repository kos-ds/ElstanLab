using System;
using System.Collections.Generic;

namespace ElstanLab.Models
{
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
    }

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

        public string VectorGroup;

        public string Cooling;


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




