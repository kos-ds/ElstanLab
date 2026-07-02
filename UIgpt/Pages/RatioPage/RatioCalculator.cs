using System;
using ElstanLab.Models;

namespace ElstanLab.Pages.RatioPage
{
    public static class RatioCalculator
    {
        //------------------------------------------------
        // Фаза AB
        //------------------------------------------------
        public static double CalcKAB(MeterPacket p)
        {
            if (p.UL2_AB == 0)
                return 0;

            return p.UL1_AB / p.UL2_AB;
        }

        //------------------------------------------------
        // Фаза BC
        //------------------------------------------------
        public static double CalcKBC(MeterPacket p)
        {
            if (p.UL2_BC == 0)
                return 0;

            return p.UL1_BC / p.UL2_BC;
        }

        //------------------------------------------------
        // Фаза CA
        //------------------------------------------------
        public static double CalcKCA(MeterPacket p)
        {
            if (p.UL2_CA == 0)
                return 0;

            return p.UL1_CA / p.UL2_CA;
        }

        public static double CalcNominalRatio(
             double baseHv,
             double baseLv,
             double hvPercent,
             double lvPercent)
        {
            double hv =
                baseHv * (1.0 + hvPercent / 100.0);

            double lv =
                baseLv * (1.0 + lvPercent / 100.0);

            if (lv == 0)
                return 0;

            return hv / lv;
        }


        //------------------------------------------------
        // Фаза A
        //------------------------------------------------

        public static double CalcKa(MeterPacket p)
        {
            if (p.U2_A == 0)
                return 0;

            return p.U1_A / p.U2_A;
        }

        //------------------------------------------------
        // Фаза B
        //------------------------------------------------

        public static double CalcKb(MeterPacket p)
        {
            if (p.U2_B == 0)
                return 0;

            return p.U1_B / p.U2_B;
        }

        //------------------------------------------------
        // Фаза C
        //------------------------------------------------

        public static double CalcKc(MeterPacket p)
        {
            if (p.U2_C == 0)
                return 0;

            return p.U1_C / p.U2_C;
        }

        //------------------------------------------------
        // Среднее
        //------------------------------------------------

        public static double CalcAverage(
            double ka,
            double kb,
            double kc)
        {
            return (ka + kb + kc) / 3.0;
        }

        //------------------------------------------------
        // Отклонение фаз
        //------------------------------------------------

        public static double CalcDeviationPercent(
            double ka,
            double kb,
            double kc,
            double avg)
        {
            double max =
                Math.Max(
                    Math.Abs(ka - avg),
                    Math.Max(
                        Math.Abs(kb - avg),
                        Math.Abs(kc - avg)));

            return max / avg * 100.0;
        }

        //------------------------------------------------
        // Ошибка от номинала
        //------------------------------------------------

        public static double CalcErrorPercent(
            double measured,
            double nominal)
        {
            if (nominal == 0)
                return 0;

            return
                (measured - nominal)
                / nominal
                * 100.0;
        }
    }
}