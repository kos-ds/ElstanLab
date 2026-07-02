namespace ElstanLab.Pages.RatioPage
{
    public class RatioGridRow
    {
        //------------------------------------------------
        // Выбор
        //------------------------------------------------

        public bool Selected;

        //------------------------------------------------
        // Режим
        //------------------------------------------------

        public string PositionName;

        public double HvPercent;

        public double LvPercent;

        //------------------------------------------------
        // Измерения
        //------------------------------------------------

        public double Ka;
        public double Kb;
        public double Kc;

        public double Kavg;

        public double DeviationPercent;

        public double ErrorPercent;

        //------------------------------------------------
        // IEC
        //------------------------------------------------

        public bool Passed;
    }
}