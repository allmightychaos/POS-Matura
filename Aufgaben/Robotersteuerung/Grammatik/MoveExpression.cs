using System.Windows;
using AbcRobotCore;

namespace Robotersteuerung.Grammatik
{
    public class MoveExpression : IExpression
    {
        string richtung;

        public MoveExpression(string richtung)
        {
            this.richtung = richtung;
        }

        public void Interpret(Rucksack context)
        {
            RobotField.Direction direction = new();

            switch (richtung)
            {
                case "UP":
                    direction = RobotField.Direction.Up;
                    break;
                case "DOWN":
                    direction = RobotField.Direction.Down;
                    break;
                case "LEFT":
                    direction = RobotField.Direction.Left;
                    break;
                case "RIGHT":
                    direction = RobotField.Direction.Right;
                    break;

                default:
                    throw new Exception($"Ungültige Richtung: {richtung}!");
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                context.field.Move(direction);
            });

            Thread.Sleep(1000);
        }
    }
}
