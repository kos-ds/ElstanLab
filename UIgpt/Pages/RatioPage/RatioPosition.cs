using System.Collections.Generic;

namespace ElstanLab.Pages.RatioPage
{
    public class RatioPosition
    {
        //------------------------------------------------
        // Отводы
        //------------------------------------------------

        public double HvPercent;

        public double LvPercent;

        //------------------------------------------------
        // Отображение
        //------------------------------------------------

        public string DisplayName
        {
            get
            {
                return string.Format(
                    "[{0:+0.##;-0.##;0}%]-[{1:+0.##;-0.##;0}%]",
                    HvPercent,
                    LvPercent);
            }
        }

        //------------------------------------------------
        // Номинальный КТР
        //------------------------------------------------

        public double NominalRatio;

      
    }
}