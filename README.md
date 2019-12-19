![Ah, gl lines](https://cdn.discordapp.com/attachments/377316629220032523/576127655712391198/unknown.png)
# Gizmos
Used for drawing runtime gizmos in builds and editor from any context in the code. It was created when I realized that the built in unity gizmos (Gizmos.DrawWireSphere, etc) don't render in builds, so I made this utility to be able to use.

*Note: Currently doesn't work if a render pipline asset is set in the graphics settings of the project in SRP, if the version of the SRP package is below 7.1.1*

## Requirements
- .NET Framework 4.5
- Git

## Installation
To install for local use, download this repo and copy everything from this repository to `<YourUnityProject>/Packages/Popcron Gizmos` folder.

If using 2018.3.x or higher, you can add a new entry to the manifest.json file in your Packages folder:
```json
"com.popcron.gizmos": "https://github.com/popcron/gizmos.git"
```

The package checks for updates every time a compile happens, and it will say so under the `Popcron/Gizmos/Update` menu if one is available, upon pressing it will make unity update the package to the latest version thats here on github. Though you can always edit it yourself if you'd like. 

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

        //toggle gizmo drawing
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Gizmos.Enabled = !Gizmos.Enabled;
        }
        
        //can also draw from update
        Gizmos.Cone(transform.position, transform.rotation, 15f, 45f, Color.green);
    }
}
```

## Camera render filter
By default, gizmos will only be drawn to the MainCamera, and the Scene view camera. If you'd like to specify other cameras to also render the gizmos, the `Gizmos.CameraFilter` predicate allows you to subscribe to a delegate where you can manually specify if a camera should be rendered to or not.

<details><summary>Example</summary>
<p>
    
```cs
private void OnEnable()
{
    //always sub in on enable, because OnEnable gets called when code gets recompiled AND on awake
    Gizmos.CameraFilter += cam =>
    {
        return cam.name == "MyOtherCamera";
    };
}
```

</p>
</details>

## Custom drawers
The ability to add custom drawers is possible. Inherit from the `Drawer` class and implement the `Draw` method. To see an example of drawing a line using a custom drawer, look at the `LineDrawer.cs` file.

The `Draw` method takes in a ref parameter to a Vector3 array as the buffer, and then a params array of objects. The method expects to return the number of points allocated to the buffer. For example, if renderering a single line, allocate the two points at buffer[0] and buffer[1], and return 2. If the number returned is not the same as the amount of points actually used, then the end result of the drawn element will look incorrect and corrupt.

## API
- `Gizmos.Line` = Draws a line from point a to b. Equivalent to [Gizmos.DrawLine](https://docs.unity3d.com/ScriptReference/Gizmos.DrawLine.html)
- `Gizmos.Square` = Draws a 2D square in the XY plane
- `Gizmos.Cube` = Draws a 3D cube in world space with orientation and scale parameters. Equivalent to [Gizmos.DrawWireCube](https://docs.unity3d.com/ScriptReference/Gizmos.DrawWireCube.html)
- `Gizmos.Bounds` = Draws a representation of a Bounds object
- `Gizmos.Rect` = Draws a rect object to a specific camera
- `Gizmos.Cone` = Draws a cone with specified orientation, length and angle. It looks like the spotlight gizmo
- `Gizmos.Sphere` = Draws a 3D sphere. Equivalent to [Gizmos.DrawWireSphere](https://docs.unity3d.com/ScriptReference/Gizmos.DrawWireSphere.html)
- `Gizmos.Circle` = Draws a 2D circle that is oriented to the camera by default

## Notes
The package uses the same class name as the built-in gizmo class, because of this, you will need to use an alias to point to the right class (`using Gizmos = Popcron.Gizmos`).

The alternative is to make a global class with the same name that redirects all of its calls to Popcron.Gizmos. The downside to this is that you will need to be explicit when calling the UnityEngine.Gizmos class if you ever need to. Choose your poison.

## FAQ
- **What namespace?** 'Popcron'
- **Does it work in builds?** Yes
- **Is there frustum culling?** Yes, it can be toggled with Gizmos.FrustumCulling
- **It's not rendering in game!** Check if your gizmo code is in OnDrawGizmos, because this method isnt called in builds, and ensure that Gizmos.Enabled is true
