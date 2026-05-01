namespace Robotersteuerung.Grammatik
{
    public class RepeatExpression : IExpression
    {
        int count;
        List<IExpression> body;

        public RepeatExpression(int count, List<IExpression> body)
        {
            this.count = count;
            this.body = body;
        }

        public void Interpret(Rucksack context)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (var item in body)
                {
                    item.Interpret(context);
                }
            }
        }
    }
}
