namespace ElstanLab.Models
{
    public class MeterPacket
    {
        //------------------------------------------------
        // Прибор 1
        //------------------------------------------------

        public double U1_A;
        public double U1_B;
        public double U1_C;

        public double UL1_AB;
        public double UL1_BC;
        public double UL1_CA;

        public double I1_A;
        public double I1_B;
        public double I1_C;

        public double P1_A;
        public double P1_B;
        public double P1_C;

        public double PTOTAL;

        public double F1;

        //------------------------------------------------
        // Прибор 2
        //------------------------------------------------

        public double U2_A;
        public double U2_B;
        public double U2_C;

        public double UL2_AB;
        public double UL2_BC;
        public double UL2_CA;

        public double F2;

        //------------------------------------------------
        // Состояния
        //------------------------------------------------

        public bool COMM1;

        public bool COMM2;

        public int MODE;

        public bool SNAPSHOT;
    }
}