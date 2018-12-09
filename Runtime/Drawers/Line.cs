using UnityEngine;

namespace Popcron
{
    public class Line : Drawer
    {
        public override Vector3[] Draw(params object[] args)
        {
            return new Vector3[] { (Vector3)args[0], (Vector3)args[1] };
        }
    }
}
