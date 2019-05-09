![Ah, gl lines](https://cdn.discordapp.com/attachments/377316629220032523/576127655712391198/unknown.png)
# Gizmos
Used for drawing runtime gizmos in builds and editor from any context in the code.

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
    }
}
```

## Custom drawers
The ability to add custom drawers is possible. Inherit from the `Drawer` class and implement the `Draw` method. To see an example of drawing a line using a custom drawer, look at the `LineDrawer.cs` file.

## API
- `Gizmos.Line` = Draws a line from point a to b. Equivalent to Gizmos.DrawLine
- `Gizmos.Square` = Draws a 2D square in the XY plane
- `Gizmos.Cube` = Draws a 3D cube in world space with orientation and scale parameters. Equivalent to Gizmos.DrawWireCube
- `Gizmos.Bounds` = Draws a representation of a Bounds object
- `Gizmos.Cone` = Draws a cone with specified orientation, length and angle
- `Gizmos.Sphere` = Draws a 3D sphere. Equivalent to Gizmos.DrawWireSphere
- `Gizmos.Circle` = Draws a 2D circle that is oriented to the camera by default

## Notes
The package uses the same class name as the built-in gizmo class, because of this, you will need to use an alias to point to the right class ('using Gizmos = Popcron.Gizmos').

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
