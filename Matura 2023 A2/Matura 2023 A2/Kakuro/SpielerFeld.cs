namespace Matura_2023_A2.Kakuro
{
    public class SpielerFeld
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int? num { get; set; }

        public SpielerFeld(int x, int y, int? num = null)
        {
            X = x;
            Y = y;
            this.num = num;
        }
    }
}
