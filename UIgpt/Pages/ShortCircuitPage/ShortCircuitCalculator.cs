using System;
using ElstanLab.Models;

namespace ElstanLab.Pages.ShortCircuitPage
{
    public static class ShortCircuitCalculator
    {
        //------------------------------------------------
        // Average voltage
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
        // Delta voltage
        //------------------------------------------------

        public static double CalcVoltageDelta(MeterPacket p)
        {
            double avg = (p.UL1_AB + p.UL1_BC + p.UL1_CA) / 3.0;

            double d1 = Math.Abs(p.UL1_AB - avg);
            double d2 = Math.Abs(p.UL1_BC - avg);
            double d3 = Math.Abs(p.UL1_CA - avg);

            double maxDev = Math.Max(d1, Math.Max(d2, d3));

            double deltaPercent = (maxDev / avg) * 100.0;

            return deltaPercent;
        }

        //------------------------------------------------
        // Average current
        //------------------------------------------------

        public static double CalcCurrentAvg(MeterPacket p)
        {
            return
                (p.I1_A
                + p.I1_B
                + p.I1_C)
                / 3.0;
        }

        //------------------------------------------------
        // Delta current
        //------------------------------------------------

        public static double CalcCurrentDelta(MeterPacket p)
        {
            double avg = (p.I1_A + p.I1_B + p.I1_C) / 3.0;

            double d1 = Math.Abs(p.I1_A - avg);
            double d2 = Math.Abs(p.I1_B - avg);
            double d3 = Math.Abs(p.I1_C - avg);

            double maxDev = Math.Max(d1, Math.Max(d2, d3));

            double deltaPercent = (maxDev / avg) * 100.0;

            return deltaPercent;
        }


        //------------------------------------------------
        // Uk %
        //------------------------------------------------

        public static double CalcUkPercent(
            double measuredVoltage,
            double nominalVoltage)
        {
            if (nominalVoltage == 0)
                return 0;

            return
                measuredVoltage
                / nominalVoltage
                * 100.0;
        }

        //------------------------------------------------
        // Zk
        //------------------------------------------------

        public static double CalcZk(double voltage, double current)
        {
            if (current == 0)
                return 0;

            return  voltage / (Math.Sqrt(3.0) * current);
        }

        //------------------------------------------------
        // Rk
        //------------------------------------------------

        public static double CalcRk(
            double power,
            double current)
        {
            if (current == 0)
                return 0;

            return power / (3.0 * current * current);
        }

        //------------------------------------------------
        // Xk
        //------------------------------------------------

        public static double CalcXk(double zk, double rk)
        {
            double x2 = (zk * zk) - (rk * rk);

            if (x2 < 0)
                return 0;

            return Math.Sqrt(x2);
        }

        //------------------------------------------------
        // Nominal HV current
        //------------------------------------------------

        public static double CalcNominalHVCurrent(double powerKva, double hvVoltage)
        {
            if (hvVoltage == 0)
                return 0;

            return (powerKva * 1000.0) / (Math.Sqrt(3) * hvVoltage);
        }

        //------------------------------------------------
        // Expected Uk voltage
        //------------------------------------------------

        public static double CalcExpectedUkVoltage(double hvVoltage,double ukPercent)
        {
            return hvVoltage * ukPercent / 100.0;
        }

        //------------------------------------------------
        // Recalculate losses
        //------------------------------------------------

        public static double RecalculateLosses(
            double measuredPower,
            double measuredCurrent,
            double nominalCurrent)
        {
            if (measuredCurrent == 0)
                return 0;

            double k =
                nominalCurrent
                / measuredCurrent;

            return measuredPower * k * k;
        }

        //////////////////////////////////////////////////
        // Corrected Uk
        //////////////////////////////////////////////////

        public static double RecalculateUk(
            double measuredUk,
            double measuredCurrent,
            double nominalCurrent)
        {
            if (measuredCurrent == 0)
                return 0;

            return
                measuredUk
                * (nominalCurrent
                / measuredCurrent);
        }
    }
}