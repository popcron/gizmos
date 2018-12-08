using System;
using UnityEngine;

namespace Popcron
{
    public abstract class Drawer
    {
        public abstract Vector3[] Draw(DrawInfo drawInfo);

        internal static T Create<T>() where T : class
        {
            if (typeof(T) == typeof(Line))
            {
                return new Line() as T;
            }
            else if (typeof(T) == typeof(Cube))
            {
                return new Cube() as T;
            }
            else if (typeof(T) == typeof(Sphere))
            {
                return new Sphere() as T;
            }
            else if (typeof(T) == typeof(Circle))
            {
                return new Circle() as T;
            }
            else if (typeof(T) == typeof(Polygon))
            {
                return new Polygon() as T;
            }
            else
            {
                T drawer = Activator.CreateInstance<T>();
                return drawer as T;
            }
        }
    }
}
