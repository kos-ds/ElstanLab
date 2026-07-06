using System;
using System.Collections.Generic;

namespace ElstanLab.Models
{
    public static class LabStorage
    {
        // ХХ
        public static NoLoadSnapshot CurrentNoLoad = new NoLoadSnapshot();

        public static List<NoLoadSnapshot> NoLoadSnapshots = new List<NoLoadSnapshot>();


        // КТР
        // public static KtrSnapshot CurrentKtr =
        //     new KtrSnapshot();

        // public static List<KtrSnapshot> KtrSnapshots =
        //     new List<KtrSnapshot>();


        // КЗ
         public static ShortCircuitSnapshot CurrentKz = new ShortCircuitSnapshot();

         public static List<ShortCircuitSnapshot> KzSnapshots = new List<ShortCircuitSnapshot>();
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




