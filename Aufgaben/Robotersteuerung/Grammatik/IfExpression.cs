using AbcRobotCore;

namespace Robotersteuerung.Grammatik
{
    public class IfExpression : IExpression
    {
        List<IExpression> body;
        string obstacle;
        string richtung;

        public IfExpression(List<IExpression> body, string obstacle, string richtung)
        {
            this.body = body;
            this.obstacle = obstacle;
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

            bool condintionMet = obstacle == "OBSTACLE" 
                ? context.field.IsObstacle(direction) 
                : context.field.IsLetter(obstacle, direction);

            if (condintionMet)
            {
                foreach (var item in body)
                {
                    item.Interpret(context);
                }
            }
            
        }
    }
}
