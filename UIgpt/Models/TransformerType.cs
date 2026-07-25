namespace ElstanLab.Models
{
    public class TransformerType
    {
        public string Name { get; set; }

        public double Power { get; set; }

        public double HVVoltage { get; set; }

        public double LVVoltage { get; set; }

        public double P0Loss { get; set; }

        public double PkLoss { get; set; }

        public double UkPercent { get; set; }

        public double I0Percent { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}