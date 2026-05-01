using System.Windows;

namespace Robotersteuerung.Grammatik
{
    public class CollectExpression : IExpression
    {
        public void Interpret(Rucksack context)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                context.field.Collect();
            });

            Thread.Sleep(1000);
        }
    }
}
