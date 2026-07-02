using System.Collections.Generic;

namespace ElstanLab.Pages.RatioPage
{
    public static class RatioPositionGenerator
    {
        public static List<RatioPosition> Generate(
            int hvTapCount,
            double hvStep,
            int lvTapCount,
            double lvStep)
        {
            List<RatioPosition> list
                = new List<RatioPosition>();

            //------------------------------------------------
            // Нет отводов
            //------------------------------------------------

            if (hvTapCount == 0 && lvTapCount == 0)
            {
                list.Add(new RatioPosition()
                {
                    HvPercent = 0,
                    LvPercent = 0
                });

                return list;
            }

            //------------------------------------------------
            // ВН
            //------------------------------------------------

            if (hvTapCount > 0 && lvTapCount == 0)
            {
                int center = hvTapCount / 2;

                for (int i = 0; i < hvTapCount; i++)
                {
                    double percent =
                        (i - center) * hvStep;

                    list.Add(new RatioPosition()
                    {
                        HvPercent = percent,
                        LvPercent = 0
                    });
                }

                return list;
            }

            //------------------------------------------------
            // НН
            //------------------------------------------------

            if (lvTapCount > 0 && hvTapCount == 0)
            {
                int center = lvTapCount / 2;

                for (int i = 0; i < lvTapCount; i++)
                {
                    double percent =
                        (i - center) * lvStep;

                    list.Add(new RatioPosition()
                    {
                        HvPercent = 0,
                        LvPercent = percent
                    });
                }

                return list;
            }

            //------------------------------------------------
            // Обе стороны
            //------------------------------------------------

            int hvCenter = hvTapCount / 2;
            int lvCenter = lvTapCount / 2;

            for (int h = 0; h < hvTapCount; h++)
            {
                double hvPercent =
                    (h - hvCenter) * hvStep;

                for (int l = 0; l < lvTapCount; l++)
                {
                    double lvPercent =
                        (l - lvCenter) * lvStep;

                    list.Add(new RatioPosition()
                    {
                        HvPercent = hvPercent,
                        LvPercent = lvPercent
                    });
                }
            }

            return list;
        }
    }
}