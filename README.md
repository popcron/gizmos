![Ah, gl lines](https://cdn.discordapp.com/attachments/377316629220032523/576127655712391198/unknown.png)
# Gizmos
Used for drawing runtime gizmos in builds and editor from any context in the code.

*Note: Currently doesn't work if a render pipline asset is set in the graphics settings of the project in SRP if its version is below 7.1.1. In addition to that, 2019.3.x is the only version that currently supports SRP 7.1.1*

## Requirements
- .NET Framework 4.5
- Git

## Installation
To install for use in Unity, copy everything from this repository to `<YourUnityProject>/Packages/Popcron.Gizmos` folder.

If using 2018.3.x or higher, you can add a new entry to the manifest.json file in your Packages folder:
```json
"com.popcron.gizmos": "https://github.com/popcron/gizmos.git"
```

## Example
```cs
using UnityEngine;
using Gizmos = Popcron.Gizmos;

[ExecuteAlways]
public class GizmoDrawer : MonoBehaviour
{
    public Material material = null;

    //this will draw in scene view and in game view, in both play mode and edit mode
    private void OnRenderObject()
    {
        //draw a line from the position of the object, to world center
        //with the color green, and dashed as well
        Gizmos.Line(transform.position, Vector3.one, Color.green, true);

        //draw a cube at the position of the object, with the correct rotation and scale
        Gizmos.Cube(transform.position, transform.rotation, transform.lossyScale);
    }

    private void Update()
    {
        //use custom material, if null it uses a default line material
        Gizmos.Material = material;

        //toggle gizmo drawing using the same key as in minecraft
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Gizmos.Enabled = !Gizmos.Enabled;
        }
        
        //can also draw from update
        Gizmos.Cone(transform.position, transform.rotation, 15f, 45f, Color.green);
    }
}
```

## Custom drawers
The ability to add custom drawers is possible. Inherit from the `Drawer` class and implement the `Draw` method. To see an example of drawing a line using a custom drawer, look at the `LineDrawer.cs` file.

The `Draw` method takes in a ref parameter to a Vector3 array as the buffer, and then a params array of objects. The method expects to return the number of points allocated to the buffer. For example, if renderering a single line, allocate the two points at buffer[0] and buffer[1], and return 2. If the number returned is not the same as the amount of points actually used, then the end result of the drawn element will look incorrect and corrupt.

## API
- `Gizmos.Line` = Draws a line from point a to b. Equivalent to [Gizmos.DrawLine](https://docs.unity3d.com/ScriptReference/Gizmos.DrawLine.html)
- `Gizmos.Square` = Draws a 2D square in the XY plane
- `Gizmos.Cube` = Draws a 3D cube in world space with orientation and scale parameters. Equivalent to [Gizmos.DrawWireCube](https://docs.unity3d.com/ScriptReference/Gizmos.DrawWireCube.html)
- `Gizmos.Bounds` = Draws a representation of a Bounds object
- `Gizmos.Cone` = Draws a cone with specified orientation, length and angle
- `Gizmos.Sphere` = Draws a 3D sphere. Equivalent to [Gizmos.DrawWireSphere](https://docs.unity3d.com/ScriptReference/Gizmos.DrawWireSphere.html)
- `Gizmos.Circle` = Draws a 2D circle that is oriented to the camera by default

## Notes
The package uses the same class name as the built-in gizmo class, because of this, you will need to use an alias to point to the right class (`using Gizmos = Popcron.Gizmos`).

The gizmos will only be processed on the scene view camera, and the default MainCamera. To change this, you can specify using the static property for `Camera` in the `Gizmo` class:
```cs
//draw for this camera instead of using the MainCamera
Gizmos.Camera = myCamera;
```

The alternative is to make a global class with the same name that redirects all of its calls to Popcron.Gizmos. The downside to this is that you will need to be explicit when calling the UnityEngine.Gizmos class. Choose your poison.

## FAQ
- **What namespace?** 'Popcron'
- **Does it work in builds?** Yes
- **Is there frustum culling?** Yes
