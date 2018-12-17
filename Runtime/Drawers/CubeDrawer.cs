using UnityEngine;

namespace Popcron.Gizmos
{
    public class CubeDrawer : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector3 position = (Vector3)values[0];
            Quaternion rotation = (Quaternion)values[1];
            Vector3 size = (Vector3)values[2];

            Vector3 point1 = new Vector3(position.x - size.x, position.y - size.y, position.z - size.z);
            Vector3 point2 = new Vector3(position.x + size.x, position.y - size.y, position.z - size.z);
            Vector3 point3 = new Vector3(position.x + size.x, position.y + size.y, position.z - size.z);
            Vector3 point4 = new Vector3(position.x - size.x, position.y + size.y, position.z - size.z);

            Vector3 point5 = new Vector3(position.x - size.x, position.y - size.y, position.z + size.z);
            Vector3 point6 = new Vector3(position.x + size.x, position.y - size.y, position.z + size.z);
            Vector3 point7 = new Vector3(position.x + size.x, position.y + size.y, position.z + size.z);
            Vector3 point8 = new Vector3(position.x - size.x, position.y + size.y, position.z + size.z);

            point1 = rotation * (point1 - position);
            point1 += position;

            point2 = rotation * (point2 - position);
            point2 += position;

            point3 = rotation * (point3 - position);
            point3 += position;

            point4 = rotation * (point4 - position);
            point4 += position;

            point5 = rotation * (point5 - position);
            point5 += position;

            point6 = rotation * (point6 - position);
            point6 += position;

            point7 = rotation * (point7 - position);
            point7 += position;

            point8 = rotation * (point8 - position);
            point8 += position;

            Vector3[] lines = new Vector3[4 * 3 * 2];

            //square
            lines[0] = point1;
            lines[1] = point2;

            lines[2] = point2;
            lines[3] = point3;

            lines[4] = point3;
            lines[5] = point4;

            lines[6] = point4;
            lines[7] = point1;

            //other square
            lines[8] = point5;
            lines[9] = point6;

            lines[10] = point6;
            lines[11] = point7;

            lines[12] = point7;
            lines[13] = point8;

            lines[14] = point8;
            lines[15] = point5;

            //connector
            lines[16] = point1;
            lines[17] = point5;

            lines[18] = point2;
            lines[19] = point6;

            lines[20] = point3;
            lines[21] = point7;

            lines[22] = point4;
            lines[23] = point8;

            return lines;
        }
    }
}
