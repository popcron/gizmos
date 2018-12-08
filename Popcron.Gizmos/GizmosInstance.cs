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
                Vector3[] lines = elements[e].Draw();
                for (int i = 0; i < lines.Length; i++)
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