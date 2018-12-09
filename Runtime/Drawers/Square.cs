using UnityEngine;

namespace Popcron
{
    public class Square : Drawer
    {
        public override Vector3[] Draw(DrawInfo drawInfo)
        {
            Vector2 position = drawInfo.vectors[0];
            Vector2 size = drawInfo.vectors[1];
            Quaternion rotation = drawInfo.rotation ?? Quaternion.identity;

            Vector2 point1 = new Vector3(position.x - size.x / 2f, position.y - size.y / 2f);
            Vector2 point2 = new Vector3(position.x + size.x / 2f, position.y - size.y / 2f);
            Vector2 point3 = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f);
            Vector2 point4 = new Vector3(position.x - size.x / 2f, position.y + size.y / 2f);

            point1 = rotation * (point1 - position);
            point1 += position;

            point2 = rotation * (point2 - position);
            point2 += position;

            point3 = rotation * (point3 - position);
            point3 += position;

            point4 = rotation * (point4 - position);
            point4 += position;

            Vector3[] lines = new Vector3[5];

            //square
            lines[0] = point1;
            lines[1] = point2;
            lines[2] = point3;
            lines[3] = point4;

            //loop back to start
            lines[4] = point1;

            return lines;
        }
    }
}
