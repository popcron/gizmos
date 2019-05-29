using System;
using UnityEngine;

namespace Popcron
{
    public class Gizmos
    {
        private static bool? _enabled = null;
        private static float? _dashGap = null;
        private static bool? _cull = null;
        private static Vector3? _offset = null;
        private static Camera _camera = null;

        private static Plane[] cameraPlanes = new Plane[6];
        private static Vector3[] buffer = new Vector3[4096];

        /// <summary>
        /// Toggles wether the gizmos could be drawn or not
        /// </summary>
        public static bool Enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = PlayerPrefs.GetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Enabled", 1) == 1;
                }

                return _enabled.Value;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    PlayerPrefs.SetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Enabled", value ? 1 : 0);
                }
            }
        }

        /// <summary>
        /// The size of the gap when drawing dashed elements
        /// </summary>
        public static float DashGap
        {
            get
            {
                if (_dashGap == null)
                {
                    _dashGap = PlayerPrefs.GetFloat(Application.buildGUID + Constants.UniqueIdentifier + ".DashGap", 0.1f);
                }

                return _dashGap.Value;
            }
            set
            {
                if (_dashGap != value)
                {
                    _dashGap = value;
                    PlayerPrefs.SetFloat(Application.buildGUID + Constants.UniqueIdentifier + ".DashGap", value);
                }
            }
        }

        /// <summary>
        /// The camera to use when rendering, uses the MainCamera by default
        /// </summary>
        public static Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
            set
            {
                _camera = value;
            }
        }

        [Obsolete("This property is obsolete. Use FrustumCulling instead.", false)]
        public static bool Cull
        {
            get
            {
                return FrustumCulling;
            }
            set
            {
                FrustumCulling = value;
            }
        }

        /// <summary>
        /// Should the camera not draw elements that are not visible?
        /// </summary>
        public static bool FrustumCulling
        {
            get
            {
                if (_cull == null)
                {
                    _cull = PlayerPrefs.GetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Cull", 1) == 1;
                }

                return _cull.Value;
            }
            set
            {
                if (_cull != value)
                {
                    _cull = value;
                    PlayerPrefs.SetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Cull", value ? 1 : 0);
                }
            }
        }

        /// <summary>
        /// The material being used to render
        /// </summary>
        public static Material Material
        {
            get
            {
                return GizmosInstance.Material;
            }
            set
            {
                GizmosInstance.Material = value;
            }
        }

        /// <summary>
        /// Global offset for all points. Default is (0, 0, 0)
        /// </summary>
        public static Vector3 Offset
        {
            get
            {
                const string Delim = ",";
                if (_offset == null)
                {
                    string data = PlayerPrefs.GetString(Application.buildGUID + Constants.UniqueIdentifier, 0 + Delim + 0 + Delim + 0);
                    int indexOf = data.IndexOf(Delim);
                    int lastIndexOf = data.LastIndexOf(Delim);
                    if (indexOf + lastIndexOf > 0)
                    {
                        string[] arr = data.Split(Delim[0]);
                        _offset = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                    }
                    else
                    {
                        return Vector3.zero;
                    }
                }

                return _offset.Value;
            }
            set
            {
                const string Delim = ",";
                if (_offset != value)
                {
                    _offset = value;
                    PlayerPrefs.SetString(Application.buildGUID + Constants.UniqueIdentifier, value.x + Delim + value.y + Delim + value.y);
                }
            }
        }

        private static int GetPolygonPoints(Vector3 position, float radius)
        {
            Camera currentCamera = GizmosInstance.currentRenderingCamera;
            if (currentCamera != null)
            {
                float distance = Vector3.Distance(position, currentCamera.transform.position);
                distance /= radius;

                int points = (int)(Mathf.Atan(16f / distance) * Mathf.Rad2Deg);
                if (points > 42) points = 42;
                if (points < 6) points = 6;
                return points;
            }

            return 16;
        }

        /// <summary>
        /// Draws an element onto the screen
        /// </summary>
        public static void Draw<T>(Color? color, bool dashed, params object[] args) where T : Drawer
        {
            if (!Enabled) return;

            Drawer drawer = Drawer.Get<T>();
            if (drawer != null)
            {
                Camera camera = GizmosInstance.currentRenderingCamera;
                int points = drawer.Draw(ref buffer, args);

                //use frustum culling
                if (FrustumCulling)
                {
                    bool visible = false;
                    if (camera != null)
                    {
                        //calculate the bounds of this element
                        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                        for (int i = 0; i < points; i++)
                        {
                            Vector3 p = buffer[i];
                            if (p.x > max.x) max.x = p.x;
                            if (p.y > max.y) max.y = p.y;
                            if (p.z > max.z) max.z = p.z;
                            if (p.x < min.x) min.x = p.x;
                            if (p.y < min.y) min.y = p.y;
                            if (p.z < min.z) min.z = p.z;
                        }

                        Bounds bounds = new Bounds();
                        bounds.SetMinMax(min, max);

                        GeometryUtility.CalculateFrustumPlanes(camera, cameraPlanes);
                        if (GeometryUtility.TestPlanesAABB(cameraPlanes, bounds))
                        {
                            visible = true;
                        }
                    }
                    else
                    {
                        //no current camera, assume its visible
                        visible = true;
                    }

                    if (!visible)
                    {
                        return;
                    }
                }

                //blindly call this in order to ensure that one exists
                GizmosInstance.GetOrCreate();

                //copy from buffer and add to the queue
                Vector3[] array = new Vector3[points];
                Array.Copy(buffer, array, points);
                GizmosInstance.Add(array, color, dashed);
            }
        }

        /// <summary>
        /// Draw line in world space
        /// </summary>
        public static void Line(Vector3 a, Vector3 b, Color? color = null, bool dashed = false)
        {
            Draw<LineDrawer>(color, dashed, a, b);
        }

        /// <summary>
        /// Draw square in world space
        /// </summary>
        public static void Square(Vector2 position, Vector2 size, Color? color = null, bool dashed = false)
        {
            Square(position, Quaternion.identity, size, color, dashed);
        }

        /// <summary>
        /// Draw square in world space with float diameter parameter
        /// </summary>
        public static void Square(Vector2 position, float diameter, Color? color = null, bool dashed = false)
        {
            Square(position, Quaternion.identity, Vector2.one * diameter, color, dashed);
        }

        /// <summary>
        /// Draw square in world space with a rotation parameter
        /// </summary>
        public static void Square(Vector2 position, Quaternion rotation, Vector2 size, Color? color = null, bool dashed = false)
        {
            Draw<SquareDrawer>(color, dashed, position, rotation, size);
        }

        /// <summary>
        /// Draws a cube in world space
        /// </summary>
        public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false)
        {
            Draw<CubeDrawer>(color, dashed, position, rotation, size);
        }

        /// <summary>
        /// Draws a representation of a bounding box
        /// </summary>
        public static void Bounds(Bounds bounds, Color? color = null, bool dashed = false)
        {
            Cube(bounds.center, Quaternion.identity, bounds.size, color, dashed);
        }

        public static void Cone(Vector3 position, Quaternion rotation, float length, float angle, Color? color = null, bool dashed = false)
        {
            //draw the end of the cone
            float endAngle = Mathf.Tan(angle * 0.5f * Mathf.Deg2Rad) * length;
            Vector3 forward = rotation * Vector3.forward;
            Vector3 endPosition = position + forward * length;
            int points = GetPolygonPoints(position, endAngle);
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, endPosition, points, endAngle, offset, rotation);

            //draw the 4 lines
            for (int i = 0; i < 4; i++)
            {
                float a = i * 90f * Mathf.Deg2Rad;
                Vector3 point = rotation * new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * endAngle;
                Line(position, position + point + forward * length, color, dashed);
            }
        }

        /// <summary>
        /// Draws a sphere at position with specified radius
        /// </summary>
        public static void Sphere(Vector3 position, float radius, Color? color = null, bool dashed = false)
        {
            int points = GetPolygonPoints(position, radius);
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, Quaternion.Euler(0f, 0f, 0f));
            Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, Quaternion.Euler(90f, 0f, 0f));
            Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, Quaternion.Euler(0f, 90f, 90f));
        }

        /// <summary>
        /// Draws a circle in world space with a specified rotation
        /// </summary>
        public static void Circle(Vector3 position, float radius, Quaternion rotation, Color? color = null, bool dashed = false)
        {
            int points = GetPolygonPoints(position, radius);
            float offset = 0f;
            Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, rotation);
        }

        /// <summary>
        /// Draws a circle in world space with the main camera position
        /// </summary>
        public static void Circle(Vector3 position, float radius, Color? color = null, bool dashed = false)
        {
            Camera currentCamera = GizmosInstance.currentRenderingCamera;
            Quaternion rotation = Quaternion.identity;
            if (currentCamera != null)
            {
                rotation = currentCamera.transform.rotation;
            }

            Circle(position, radius, rotation, color, dashed);
        }
    }
}
