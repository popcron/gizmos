using UnityEngine;

namespace Popcron.Gizmos
{
    public class SphereDrawer : Drawer
    {
        public override Vector3[] Draw(params object[] values)
        {
            Vector3 position = (Vector3)values[0];
            float radius = (float)values[1];

            return PolygonDrawer.Draw(position, Quaternion.identity, radius, 16, 0);
        }
    }
}
