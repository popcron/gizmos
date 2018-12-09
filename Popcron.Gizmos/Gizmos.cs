using UnityEngine;

namespace Popcron
{
    public class Gizmos
    {
        internal const string UniqueIdentifier = "Popcron.Gizmos";

        /// <summary>
        /// Toggles wether the gizmos could be drawn or not
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return PlayerPrefs.GetInt(UniqueIdentifier + ".Enabled", 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt(UniqueIdentifier + ".Enabled", value ? 1 : 0);
            }
        }

        /// <summary>
        /// Draws an element onto the screen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        public static void Draw<T>(DrawInfo info, bool dashed) where T : class
        {
            if (!Enabled) return;

            if (typeof(T).IsSubclassOf(typeof(Drawer)))
            {
                GizmosInstance.CheckInstance();

                Element item = new Element
                {
                    drawer = Drawer.Create<T>() as Drawer,
                    info = info,
                    dashed = dashed
                };

                GizmosInstance.elements.Add(item);
            }
            else
            {
                Debug.LogWarning(typeof(T) + "is not a Drawer");
            }
        }

        /// <summary>
        /// Draw line in world space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="color"></param>
        public static void Line(Vector3 a, Vector3 b, Color? color = null, bool dashed = false)
        {
            DrawInfo info = new DrawInfo();
            info.vectors.Add(a);
            info.vectors.Add(b);
            info.color = color;

            Draw<Line>(info, dashed);
        }

        /// <summary>
        /// Draw square in world space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="color"></param>
        public static void Square(Vector2 position, Vector2 size, Color? color = null, bool dashed = false)
        {
            DrawInfo info = new DrawInfo();
            info.vectors.Add(position);
            info.vectors.Add(size);
            info.color = color;

            Draw<Square>(info, dashed);
        }

        /// <summary>
        /// Draw square in world space with a rotation parameter
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="color"></param>
        public static void Square(Vector2 position, Quaternion rotation, Vector2 size, Color? color = null, bool dashed = false)
        {
            DrawInfo info = new DrawInfo();
            info.vectors.Add(position);
            info.vectors.Add(size);
            info.rotation = rotation;
            info.color = color;

            Draw<Square>(info, dashed);
        }


        /// <summary>
        /// Draws a cube in world space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false)
        {
            DrawInfo info = new DrawInfo();
            info.vectors.Add(position);
            info.vectors.Add(size);
            info.rotation = rotation;
            info.color = color;

            Draw<Cube>(info, dashed);
        }

        /// <summary>
        /// Draws a sphere in world space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Sphere(Vector3 position, float radius, Color? color = null, bool dashed = false)
        {
            DrawInfo info = new DrawInfo();
            info.vectors.Add(position);
            info.floats.Add(radius);
            info.rotation = Camera.main?.transform?.rotation;
            info.color = color;

            Draw<Sphere>(info, dashed);
        }
    }
}
