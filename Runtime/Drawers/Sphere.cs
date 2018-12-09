using UnityEngine;

namespace Popcron
{
    public class Sphere : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector3 position = (Vector3)values[0];
            float radius = (float)values[1];
            Color color = (Color)values[2];
            Quaternion rotation = (Quaternion)values[2];

            return Polygon.Draw(position, rotation, radius, 16, 0);
        }
    }
}
