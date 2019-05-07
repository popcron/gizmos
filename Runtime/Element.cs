using System;
using UnityEngine;

namespace Popcron
{
    [Serializable]
    internal class Element
    {
        public bool active = false;
        public Vector3[] points = { };
        public Color color = Color.white;
        public bool dashed = false;
    }
}