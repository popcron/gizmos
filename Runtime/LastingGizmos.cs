using UnityEngine;

namespace Popcron
{
    public static class LastingGizmos
    {
        public static int BufferSize
        {
            get => Gizmos.BufferSize;
            set => Gizmos.BufferSize = value;
        }

        /// <summary>
        /// Toggles whatever the gizmos could be drawn or not. Referencing to <see cref="Gizmos.Enabled"/>
        /// </summary>
        public static bool Enabled
        {
            get => Gizmos.Enabled;
            set => Gizmos.Enabled = value;
        }

        /// <summary>
        /// The size of the gap when drawing dashed elements.
        /// Default gap size is 0.1
        /// Referencing to <see cref="Gizmos.DashGap"/>
        /// </summary>
        public static float DashGap
        {
            get => Gizmos.DashGap;
            
            set => Gizmos.DashGap = value;
        }

        /// <summary>
        /// Should the camera not draw elements that are not visible? Referencing to <see cref="Gizmos.FrustumCulling"/>
        /// </summary>
        public static bool FrustumCulling
        {
            get => Gizmos.FrustumCulling;
            set => Gizmos.FrustumCulling = value;
        }

        /// <summary>
        /// The material being used to render
        /// </summary>
        public static Material Material
        {
            get => GizmosInstance.Material;
            set => GizmosInstance.Material = value;
        }

        /// <summary>
        /// Rendering pass to activate. Referencing to <see cref="Gizmos.Pass"/>
        /// </summary>
        public static int Pass
        {
            get => Gizmos.Pass;
            set => Gizmos.Pass = value;
        }

        /// <summary>
        /// Global offset for all points. Default is (0, 0, 0). Referencing to <see cref="Gizmos.Offset"/>
        /// </summary>
        public static Vector3 Offset
        {
            get => Gizmos.Offset;
            set => Gizmos.Offset = value;
        }

        /// <summary>
        /// Draws an element onto the screen.
        /// </summary>
        public static void Draw<T>(Color? color, bool dashed, float time = 1, params object[] args) where T : Drawer
        {
            if (!Enabled)
            {
                return;
            }

            GizmosInstance.DoForLong(() => Gizmos.Draw<T>(color, dashed, args), time);
        }

        /// <summary>
        /// Draws an array of lines. Useful for things like paths.
        /// </summary>
        public static void Lines(Vector3[] lines, Color? color = null, bool dashed = false, float time = 1)
        {
            if (!Enabled)
            {
                return;
            }

            GizmosInstance.DoForLong(() => GizmosInstance.Submit(lines, color, dashed), time);
        }

        /// <summary>
        /// Draw line in world space.
        /// </summary>
        public static void Line(Vector3 a, Vector3 b, Color? color = null, bool dashed = false, float time = 1)
        {
            Draw<LineDrawer>(color, dashed, time, a, b);
        }

        /// <summary>
        /// Draw square in world space.
        /// </summary>
        public static void Square(Vector2 position, Vector2 size, Color? color = null, bool dashed = false, float time = 1)
        {
            Square(position, Quaternion.identity, size, color, dashed);
        }

        /// <summary>
        /// Draw square in world space with float diameter parameter.
        /// </summary>
        public static void Square(Vector2 position, float diameter, Color? color = null, bool dashed = false)
        {
            Square(position, Quaternion.identity, Vector2.one * diameter, color, dashed);
        }

        /// <summary>
        /// Draw square in world space with a rotation parameter.
        /// </summary>
        public static void Square(Vector2 position, Quaternion rotation, Vector2 size, Color? color = null, bool dashed = false, float time = 1)
        {
            Draw<SquareDrawer>(color, dashed, time, position, rotation, size);
        }

        /// <summary>
        /// Draws a cube in world space.
        /// </summary>
        public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false, float time = 1)
        {
            Draw<CubeDrawer>(color, dashed, time, position, rotation, size);
        }

        /// <summary>
        /// Draws a rectangle in screen space.
        /// </summary>
        public static void Rect(Rect rect, Camera camera, Color? color = null, bool dashed = false, float time = 1)
        {
            rect.y = Screen.height - rect.y;
            Vector2 corner = camera.ScreenToWorldPoint(new Vector2(rect.x, rect.y - rect.height));
            Draw<SquareDrawer>(color, dashed, time, corner + rect.size * 0.5f, Quaternion.identity, rect.size);
        }

        /// <summary>
        /// Draws a representation of a bounding box.
        /// </summary>
        public static void Bounds(Bounds bounds, Color? color = null, bool dashed = false, float time = 1)
        {
            Draw<CubeDrawer>(color, dashed, time, bounds.center, Quaternion.identity, bounds.size);
        }

        /// <summary>
        /// Draws a cone similar to the one that spot lights draw.
        /// </summary>
        public static void Cone(Vector3 position, Quaternion rotation, float length, float angle, Color? color = null, bool dashed = false, int pointsCount = 16, float time = 1)
        {
            //draw the end of the cone
            float endAngle = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad) * length;
            Vector3 forward = rotation * Vector3.forward;
            Vector3 endPosition = position + forward * length;
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, time, endPosition, pointsCount, endAngle, offset, rotation);

            //draw the 4 lines
            for (int i = 0; i < 4; i++)
            {
                float a = i * 90f * Mathf.Deg2Rad;
                Vector3 point = rotation * new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * endAngle;
                Line(position, position + point + forward * length, color, dashed);
            }
        }

        /// <summary>
        /// Draws a sphere at position with specified radius.
        /// </summary>
        public static void Sphere(Vector3 position, float radius, Color? color = null, bool dashed = false, int pointsCount = 16, float time = 1)
        {
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, time, position, pointsCount, radius, offset, Quaternion.Euler(0f, 0f, 0f));
            Draw<PolygonDrawer>(color, dashed, time, position, pointsCount, radius, offset, Quaternion.Euler(90f, 0f, 0f));
            Draw<PolygonDrawer>(color, dashed, time, position, pointsCount, radius, offset, Quaternion.Euler(0f, 90f, 90f));
        }

        /// <summary>
        /// Draws a circle in world space and billboards towards the camera.
        /// </summary>
        public static void Circle(Vector3 position, float radius, Camera camera, Color? color = null, bool dashed = false, int pointsCount = 16, float time = 1)
        {
            float offset = 0f;
            Quaternion rotation = Quaternion.LookRotation(position - camera.transform.position);
            Draw<PolygonDrawer>(color, dashed, time, position, pointsCount, radius, offset, rotation);
        }

        /// <summary>
        /// Draws a circle in world space with a specified rotation.
        /// </summary>
        public static void Circle(Vector3 position, float radius, Quaternion rotation, Color? color = null, bool dashed = false, int pointsCount = 16, float time = 1)
        {
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, time, position, pointsCount, radius, offset, rotation);
        }
    }
}