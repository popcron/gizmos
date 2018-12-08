using UnityEngine;

namespace Popcron
{
    public class Circle : Drawer
    {
        public override Vector3[] Draw(DrawInfo drawInfo)
        {
            Vector3 position = drawInfo.vectors[0];
            float radius = drawInfo.floats.Count > 0 ? drawInfo.floats[0] : 16f;

            return Polygon.Draw(position, Quaternion.identity, radius, 16, 0);
        }
    }
}
