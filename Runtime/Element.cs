using System;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron.Gizmos
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