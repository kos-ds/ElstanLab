using ElstanLab.Models;

namespace ElstanLab.Services
{
    public static class CurrentTransformerSelector
    {
        public static CurrentTransformer Get(double current)
        {
            if (current <= 5)
            {
                return new CurrentTransformer
                {
                    Divider = 1,
                    Command = "k"
                };
            }

            if (current <= 10)
            {
                return new CurrentTransformer
                {
                    Divider = 2,
                    Command = "d"
                };
            }

            if (current <= 20)
            {
                return new CurrentTransformer
                {
                    Divider = 4,
                    Command = "s"
                };
            }

            return new CurrentTransformer
            {
                Divider = 10,
                Command = "c"
            };
        }
    }
}