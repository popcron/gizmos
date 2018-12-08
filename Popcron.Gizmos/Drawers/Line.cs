using UnityEngine;

namespace Popcron
{
    public class Line : Drawer
    {
        public override Vector3[] Draw(DrawInfo drawInfo)
        {
            Vector3 a = drawInfo.vectors[0];
            Vector3 b = drawInfo.vectors[1];

            return new Vector3[] { a, b };
        }
    }
}
