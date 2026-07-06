namespace ElstanLab.Models
{
    public class PassportModel
    {
        public string TransformerName { get; set; }

        public double Power { get; set; }

        public double HV { get; set; }

        public double LV { get; set; }

        public string VectorGroup { get; set; }

        public string SerialNumber { get; set; }

        public string Manufacturer { get; set; }

        public int Year { get; set; }

        public double NominalCurrentHV { get; set; }

        public double NominalCurrentLV { get; set; }

        public double PassportNoLoadCurrentPercent { get; set; }

        public double PassportNoLoadLosses { get; set; }

        public double PassportUkPercent { get; set; }

        public double PassportShortCircuitLosses { get; set; }
    }
}