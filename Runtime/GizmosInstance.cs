using System.Collections.Generic;
using UnityEngine;

namespace Popcron
{
    [ExecuteInEditMode]
    public class GizmosInstance : MonoBehaviour
    {
        private static GizmosInstance instance;
        private static bool hotReloaded = true;
        private static Material defaultMaterial;
        private Material overrideMaterial;

        internal static List<Element> elements = new List<Element>();

        private static GizmosInstance Instance
        {
            get
            {
                bool forceCheck = false;

                if (!Application.isPlaying)
                {
                    forceCheck = !instance;
                }

                if (hotReloaded || forceCheck)
                {
                    GameObject gameObject = GameObject.Find(Gizmos.UniqueIdentifier + ".GameObject");
                    if (!gameObject)
                    {
                        instance = new GameObject(Gizmos.UniqueIdentifier + ".GameObject").AddComponent<GizmosInstance>();
                        instance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                    }
                    else
                    {
                        instance = gameObject.GetComponent<GizmosInstance>();
                        if (!instance)
                        {
                            instance = gameObject.AddComponent<GizmosInstance>();
                        }
                    }

                    hotReloaded = false;
                }

                return instance;
            }
        }

        internal static void CheckInstance()
        {
            //dumb empty statement
            if (Instance) ;
        }

        /// <summary>
        /// The material being used to render
        /// </summary>
        public static Material Material
        {
            get
            {
                if (Instance.overrideMaterial)
                {
                    return Instance.overrideMaterial;
                }

                return DefaultMaterial;
            }
            set
            {
                Instance.overrideMaterial = value;
            }
        }

        /// <summary>
        /// The default line renderer material
        /// </summary>
        public static Material DefaultMaterial
        {
            get
            {
                if (!defaultMaterial)
                {
                    // Unity has a built-in shader that is useful for drawing
                    // simple colored things.
                    Shader shader = Shader.Find("Hidden/Internal-Colored");
                    defaultMaterial = new Material(shader)
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };

                    // Turn on alpha blending
                    defaultMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    defaultMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    // Turn backface culling off
                    defaultMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    // Turn off depth writes
                    defaultMaterial.SetInt("_ZWrite", 0);
                }

                return defaultMaterial;
            }
        }

        private void OnRenderObject()
        {
            Material.SetPass(0);

            GL.PushMatrix();
            GL.Begin(GL.LINES);

            //draw elements
            for (int e = 0; e < elements.Count; e++)
            {
                GL.Color(elements[e].Color);

                List<Vector3> lines = new List<Vector3>();

                Vector3[] elementLines = elements[e].Draw();
                if (elements[e].dashed)
                {
                    //subdivide
                    const float Interval = 2f;
                    for (int i = 0; i < elementLines.Length; i++)
                    {
                        Vector3 pointA = elementLines[i];
                        Vector3 pointB = elementLines[(i + 1) % elementLines.Length];

                        int amount = Mathf.RoundToInt(Vector3.Distance(pointA, pointB) / Interval);
                        Vector3 direction = (pointB - pointA).normalized;
                        for (int a = 0; a < amount - 1; ++a)
                        {
                            if (a % 2 == 0)
                            {
                                float lerp = a / ((float)amount - 1);
                                Vector3 start = Vector3.Lerp(pointA, pointB, lerp);
                                Vector3 end = start + (direction * Interval);
                                lines.Add(start);
                                lines.Add(end);
                            }
                        }
                    }
                }
                else
                {
                    lines.AddRange(elementLines);
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    GL.Vertex(lines[i]);
                }
            }

            //clear elemenets
            elements.Clear();

            GL.End();
            GL.PopMatrix();
        }
    }
}