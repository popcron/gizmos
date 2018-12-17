using UnityEngine;

namespace Popcron.Gizmos
{
    public class LineDrawer : Drawer
    {
        public override Vector3[] Draw(params object[] args)
        {
            return new Vector3[] { (Vector3)args[0], (Vector3)args[1] };
        }
    }
}
