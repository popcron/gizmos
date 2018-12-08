# Gizmos
Used for drawing runtime gizmos in builds and editor from any context in the code.

## Requirements
- .NET Framework 3.5

## Example
```cs
using UnityEngine;

using Gizmos = Popcron.Gizmos;

public class GizmoDrawer : MonoBehaviour
{
    private void Update()
    {
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
