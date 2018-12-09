using UnityEngine;

namespace Popcron
{
    public class Polygon : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector3 position = (Vector3)values[0];
            int points = (int)values[1];
            float radius = (float)values[2];
            float offset = (float)values[3];
            Quaternion rotation = (Quaternion)values[3];

            return Draw(position, rotation, radius, points, offset);
        }

        internal static Vector3[] Draw(Vector3 position, Quaternion rotation, float radius, int points, float offset)
        {
            float angle = 360f / points;
            offset *= Mathf.Deg2Rad;

            Vector3[] lines = new Vector3[points + 1];
            for (int i = 0; i < points; i++)
            {
                float cx = Mathf.Cos(Mathf.Deg2Rad * angle * i + offset) * radius;
                float cy = Mathf.Sin(Mathf.Deg2Rad * angle * i + offset) * radius;
                Vector3 current = rotation * new Vector3(cx, cy);

                //float nx = Mathf.Cos(Mathf.Deg2Rad * angle * (i + 1) + offset) * radius;
                //float ny = Mathf.Sin(Mathf.Deg2Rad * angle * (i + 1) + offset) * radius;
                //Vector3 next = rotation * new Vector3(nx, ny);

                lines[i] = position + current;
            }

            return lines;
        }
    }
}
