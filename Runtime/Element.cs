using System;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron
{
    [Serializable]
    internal struct Element
    {
        public Drawer drawer;
        public DrawInfo info;
        public bool dashed;

        public Color Color
        {
            get
            {
                if (info == null) return Color.white;

                return info.color ?? Color.white;
            }
        }

        public Vector3[] Draw()
        {
            if (drawer == null) return null;

            return drawer.Draw(info);
        }
    }
}