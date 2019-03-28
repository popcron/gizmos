using System;
using System.Collections.Generic;
using UnityEngine;

using Popcron.Gizmos;

public class Gizmos
{
    private static bool? enabled = null;
	private static bool? cull = null;
    private static Vector3? offset = null;
    private static Camera camera = null;

    /// <summary>
    /// Toggles wether the gizmos could be drawn or not
    /// </summary>
    public static bool Enabled
    {
        get
        {
            if (enabled == null)
            {
                enabled = PlayerPrefs.GetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Enabled", 1) == 1;
            }

            return enabled.Value;
        }
        set
        {
            if (enabled != value)
            {
                enabled = value;
                PlayerPrefs.SetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Enabled", value ? 1 : 0);
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
            if (camera == null)
            {
                camera = Camera.main;
            }

            return camera;
        }
        set
        {
            camera = value;
        }
    }

    /// <summary>
    /// Should the camera not draw elements that are not visible?
    /// </summary>
    public static bool Cull
    {
        get
        {
            if (cull == null)
            {
                cull = PlayerPrefs.GetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Cull", 1) == 1;
            }

            return cull.Value;
        }
        set
        {
            if (cull != value)
            {
                cull = value;
                PlayerPrefs.SetInt(Application.buildGUID + Constants.UniqueIdentifier + ".Cull", value ? 1 : 0);
            }
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
            if (offset == null)
            {
                string data = PlayerPrefs.GetString(Application.buildGUID + Constants.UniqueIdentifier, 0 + Delim + 0 + Delim + 0);
                int indexOf = data.IndexOf(Delim);
                int lastIndexOf = data.LastIndexOf(Delim);
                if (indexOf + lastIndexOf > 0)
                {
                    string[] arr = data.Split(Delim[0]);
                    offset = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                }
                else
                {
                    return Vector3.zero;
                }
            }

            return offset.Value;
        }
        set
        {
            const string Delim = ",";
            if (offset != value)
            {
                offset = value;
                PlayerPrefs.SetString(Application.buildGUID + Constants.UniqueIdentifier, value.x + Delim + value.y + Delim + value.y);
            }
        }
    }

    /// <summary>
    /// Draws an element onto the screen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    public static void Draw<T>(Color? color, bool dashed, params object[] args) where T : Drawer
    {
        if (!Enabled) return;

        Drawer drawer = Drawer.Get<T>();
        if (drawer != null)
        {
            Camera currentCamera = GizmosInstance.currentCamera;
            Vector3[] points = drawer.Draw(args);

			if (Cull)
			{
				bool visible = false;
				for (int i = 0; i < points.Length; i++)
				{
                    //no current camera, assume its visible
                    if (currentCamera == null)
                    {
                        visible = true;
                        break;
                    }

                    //frustrum cull using the current camera
                    Vector3 p = currentCamera.WorldToScreenPoint(points[i]);
					if (p.x >= 0 && p.y >= 0 && p.x <= Screen.width && p.y <= Screen.height && p.z >= 0)
					{
						visible = true;
						break;
					}
				}

				if (!visible)
				{
					return;
				}
			}

            GizmosInstance.Add(points, color, dashed);
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
        Draw<LineDrawer>(color, dashed, a, b);
    }

    /// <summary>
    /// Draw square in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="color"></param>
    public static void Square(Vector2 position, Vector2 size, Color? color = null, bool dashed = false)
    {
        Draw<SquareDrawer>(color, dashed, position, Quaternion.identity, size * 0.5f);
    }

    /// <summary>
    /// Draw square in world space with float diameter parameter
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="color"></param>
    public static void Square(Vector2 position, float diameter, Color? color = null, bool dashed = false)
    {
        Draw<SquareDrawer>(color, dashed, position, Quaternion.identity, Vector2.one * diameter * 0.5f);
    }

    /// <summary>
    /// Draw square in world space with a rotation parameter
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="color"></param>
    public static void Square(Vector2 position, Quaternion rotation, Vector2 size, Color? color = null, bool dashed = false)
    {
        Draw<SquareDrawer>(color, dashed, position, rotation, size * 0.5f);
    }

    /// <summary>
    /// Draws a cube in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false)
    {
        Draw<CubeDrawer>(color, dashed, position, rotation, size * 0.5f);
    }

    /// <summary>
    /// Draws a circle in world space with the main camera position
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Circle(Vector3 position, float radius, Color? color = null, bool dashed = false)
    {
        int points = 16;
        float offset = 0f;
        Quaternion rotation = Camera.transform.rotation;
        Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, rotation);
    }

    /// <summary>
    /// Draws a circle in world space with a specified rotation
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Circle(Vector3 position, float radius, Quaternion rotation, Color? color = null, bool dashed = false)
    {
        int points = 16;
        float offset = 0f;
        Draw<PolygonDrawer>(color, dashed, position, points, radius, offset, rotation);
    }
}
