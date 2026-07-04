using System;
using ElstanLab.Models;

namespace ElstanLab.Pages.NoLoadPage
{
    public static class NoLoadCalculator
    {
        //------------------------------------------------
        // Voltage average
        //------------------------------------------------

        public static double CalcVoltageAvg(
            MeterPacket p)
        {
            return
                (p.UL1_AB
                + p.UL1_BC
                + p.UL1_CA)
                / 3.0;
        }

        //------------------------------------------------
        // Current average
        //------------------------------------------------

        public static double CalcCurrentAvg(
            MeterPacket p)
        {
            return
                (p.I1_A
                + p.I1_B
                + p.I1_C)
                / 3.0;
        }

        //------------------------------------------------
        // Nominal current
        //------------------------------------------------

        public static double CalcNominalCurrent(
            double powerKva,
            double voltage)
        {
            if (voltage == 0)
                return 0;

            return
                (powerKva * 1000.0)
                / (Math.Sqrt(3) * voltage);
        }

        //------------------------------------------------
        // I0 %
        //------------------------------------------------

        public static double CalcI0Percent(
            double current,
            double nominalCurrent)
        {
            if (nominalCurrent == 0)
                return 0;

            return
                current
                / nominalCurrent
                * 100.0;
        }

        //------------------------------------------------
        // Cos phi
        //------------------------------------------------

        public static double CalcCosPhi(
            double power,
            double voltage,
            double current)
        {
            double s =
                Math.Sqrt(3)
                * voltage
                * current;

            if (s == 0)
                return 0;

            return power / s;
        }

        //////////////////////////////////////////////////
        // Delta current %
        //////////////////////////////////////////////////

        public static double CalcDeltaCurrent(
            MeterPacket p,
            double avg)
        {
            if (avg == 0)
                return 0;

            double max =
                Math.Max(
                    Math.Abs(p.I1_A - avg),
                    Math.Max(
                        Math.Abs(p.I1_B - avg),
                        Math.Abs(p.I1_C - avg)));

            return max / avg * 100.0;
        }

        //////////////////////////////////////////////////
        // Delta voltage %
        //////////////////////////////////////////////////

        public static double CalcDeltaVoltage(
            MeterPacket p,
            double avg)
        {
            if (avg == 0)
                return 0;

            double max =
                Math.Max(
                    Math.Abs(p.UL1_AB - avg),
                    Math.Max(
                        Math.Abs(p.UL1_BC - avg),
                        Math.Abs(p.UL1_CA - avg)));

            return max / avg * 100.0;
        }
    }
}