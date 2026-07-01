namespace ElstanLab.Models
{
    public static class ModeNames
    {
        public static string Get(LabMode mode)
        {
            switch (mode)
            {
                case LabMode.Passport:
                    return "Паспорт изделия";

                case LabMode.KTR:
                    return "Коэффициент трансформации";

                case LabMode.ShortCircuit:
                    return "Короткое замыкание";

                case LabMode.NoLoad:
                    return "Холостой ход";

                case LabMode.IVW:
                    return "Индуцированное напряжение";

                case LabMode.Other:
                    return "Дополнительные испытания";

                case LabMode.Report:
                    return "Отчет";

                default:
                    return "Неизвестно";
            }
        }
    }
}