using System;
using System.Collections.Generic;
using UnityEngine;

using Popcron.Gizmos;

public class Gizmos
{
    private static bool? enabled = null;

    /// <summary>
    /// Toggles wether the gizmos could be drawn or not
    /// </summary>
    public static bool Enabled
    {
        get
        {
            if (enabled == null)
            {
                enabled = PlayerPrefs.GetInt(Application.buildGUID + Constants.UniqueIdentifier, 1) == 1;
            }

            return enabled.Value;
        }
        set
        {
            if (enabled != value)
            {
                enabled = value;
                PlayerPrefs.SetInt(Application.buildGUID + Constants.UniqueIdentifier, value ? 1 : 0);
            }
        }
    }

    /// <summary>
    /// Draws an element onto the screen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    public static void Draw<T>(Color? color = null, bool dashed = false, params object[] args) where T : Drawer
    {
        if (!Enabled) return;

        Drawer drawer = Drawer.Get<T>();
        if (drawer != null)
        {
            Vector3[] points = drawer.Draw(args);
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
        Draw<SquareDrawer>(color, dashed, position, Quaternion.identity, size);
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
        Draw<SquareDrawer>(color, dashed, position, rotation, size);
    }

    /// <summary>
    /// Draws a cube in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Cube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool dashed = false)
    {
        Draw<CubeDrawer>(color, dashed, position, rotation, size);
    }

    /// <summary>
    /// Draws a sphere in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Sphere(Vector3 position, float radius, Color? color = null, bool dashed = false)
    {
        Draw<SphereDrawer>(color, dashed, position, radius);
    }
}