using UnityEngine;

namespace Popcron
{
    public class Sphere : Drawer
    {
        public override Vector3[] Draw(DrawInfo drawInfo)
        {
            Vector3 position = drawInfo.vectors[0];
            float radius = drawInfo.floats.Count > 0 ? drawInfo.floats[0] : 16f;
            Color color = drawInfo.color ?? Color.white;
            Quaternion rotation = drawInfo.rotation ?? Camera.main.transform.rotation;

            return Polygon.Draw(position, rotation, radius, 16, 0);
        }
    }
}
