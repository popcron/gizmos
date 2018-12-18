using UnityEngine;

namespace Popcron.Gizmos
{
    public class SquareDrawer : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector2 position = (Vector2)values[0];
            Quaternion rotation = (Quaternion)values[1];
            Vector2 size = (Vector2)values[2];

            Vector2 point1 = new Vector3(position.x - size.x, position.y - size.y);
            Vector2 point2 = new Vector3(position.x + size.x, position.y - size.y);
            Vector2 point3 = new Vector3(position.x + size.x, position.y + size.y);
            Vector2 point4 = new Vector3(position.x - size.x, position.y + size.y);

            point1 = rotation * (point1 - position);
            point1 += position;

            point2 = rotation * (point2 - position);
            point2 += position;

            point3 = rotation * (point3 - position);
            point3 += position;

            point4 = rotation * (point4 - position);
            point4 += position;

            Vector3[] lines = new Vector3[8];

            //square
            lines[0] = point1;
            lines[1] = point2;

            lines[2] = point2;
            lines[3] = point3;

            lines[4] = point3;
            lines[5] = point4;

            //loop back to start
            lines[6] = point4;
            lines[7] = point1;

            return lines;
        }
    }
}
