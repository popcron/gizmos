# Gizmos
Used for drawing runtime gizmos in builds and editor from any context in the code.

## Requirements
- .NET Framework 3.5

## Installation
Add the .dll file to the Plugins folder.

If using 2018.3.x, you can add a new entry to the manifest.json file in your Packages folder:
```json
"com.popcron.gizmos": "https://github.com/popcron/gizmos.git#unity"
```

## Example
```cs
using UnityEngine;

using Gizmos = Popcron.Gizmos;

public class GizmoDrawer : MonoBehaviour
{
    public Material material = null;

    private void Update()
    {
        //use custom material, if null it uses a default line material
        Gizmos.Material = material;
        
        //toggle gizmo drawing using the same key as in minecwaft
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Gizmos.Enabled = !Gizmos.Enabled;
        }
        
        //draw a line from the position of the object, to world center
        Gizmos.Line(transform.position, Vector3.one);
        
        //draw a cube at the position of the object, with the correct rotation and scale
        Gizmos.Cube(transform.position, transform.rotation, transform.lossyScale);
    }
}
```

## Custom drawers
The ability to add custom drawers is possible. Inherit from the `Drawer` class and implement the `Draw` method. To see an example of drawing a line using a custom drawer, look at the `Line.cs` file.

## FAQ
- **What namespace?** Popcron
- **Does it work in builds?** Yes
