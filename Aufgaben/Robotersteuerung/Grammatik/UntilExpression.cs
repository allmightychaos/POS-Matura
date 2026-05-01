using AbcRobotCore;

namespace Robotersteuerung.Grammatik
{
    public class UntilExpression : IExpression
    {
        List<IExpression> body;
        string obstacle;
        string richtung;

        public UntilExpression(List<IExpression> body, string obstacle, string richtung)
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

            /*
            if (context.field.IsObstacle(direction) || context.field.IsLetter(obstacle, direction))
            {
                foreach (var item in body)
                {
                    item.Interpret(context);
                }
            }*/

            bool isDone() => obstacle == "OBSTACLE"
                    ? context.field.IsObstacle(direction)
                    : context.field.IsLetter(obstacle, direction);

            while (!isDone())
            {
                foreach (var item in body)
                {
                    item.Interpret(context);
                }
            }
        }
    }
}
