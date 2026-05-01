namespace Robotersteuerung.Grammatik
{
    public class ProgramExpression : IExpression
    {
        List<IExpression> body;

        public ProgramExpression(List<IExpression> body)
        {
            this.body = body;
        }

        public void Interpret(Rucksack context)
        {
            foreach (var item in body)
            {
                item.Interpret(context);
            }
        }
    }
}
