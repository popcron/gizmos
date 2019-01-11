using UnityEngine;

namespace Popcron.Gizmos
{
    public class PolygonDrawer : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector3 position = (Vector3)values[0];
            int points = (int)values[1];
            float radius = (float)values[2];
            float offset = (float)values[3];
            Quaternion rotation = (Quaternion)values[4];

            float step = 360f / points;
            offset *= Mathf.Deg2Rad;

            Vector3[] lines = new Vector3[points * 2];
            for (int i = 0; i < points; i++)
            {
                float cx = Mathf.Cos(Mathf.Deg2Rad * step * i + offset) * radius * 0.5f;
                float cy = Mathf.Sin(Mathf.Deg2Rad * step * i + offset) * radius * 0.5f;
                Vector3 current = new Vector3(cx, cy);

                float nx = Mathf.Cos(Mathf.Deg2Rad * step * (i + 1) + offset) * radius * 0.5f;
                float ny = Mathf.Sin(Mathf.Deg2Rad * step * (i + 1) + offset) * radius * 0.5f;
                Vector3 next = new Vector3(nx, ny);

                lines[i * 2] = position + (rotation * current);
                lines[(i * 2) + 1] = position + (rotation * next);
            }

            return lines;
        }
    }
}
