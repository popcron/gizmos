using System;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron.Gizmos
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class GizmosInstance : MonoBehaviour
    {
        private const int DefaultQueueSize = 128;

        private static GizmosInstance instance;
        private static bool hotReloaded = true;
        private static Material defaultMaterial;

        private Material overrideMaterial;
        private int queueIndex = 0;
        private Element[] queue = new Element[DefaultQueueSize];

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
                    GameObject gameObject = GameObject.Find(Constants.UniqueIdentifier + ".GameObject");
                    if (!gameObject)
                    {
                        instance = new GameObject(Constants.UniqueIdentifier + ".GameObject").AddComponent<GizmosInstance>();
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

        internal static void Add(Vector3[] points, Color? color = null, bool dashed = false)
        {
            var instance = Instance;

            //excedeed the length, so loopback
            if (instance.queueIndex >= DefaultQueueSize)
            {
                instance.queueIndex = 0;
            }
            
            instance.queue[instance.queueIndex].active = true;
            instance.queue[instance.queueIndex].color = color ?? Color.white;
            instance.queue[instance.queueIndex].points = points;
            instance.queue[instance.queueIndex].dashed = dashed;

            instance.queueIndex++;
        }

        private void OnEnable()
        {
            //populate queue with empty elements
            queue = new Element[DefaultQueueSize];
            for (int i = 0; i < DefaultQueueSize; i++)
            {
                queue[i] = new Element();
            }

            Camera.onPostRender += OnRendered;
        }

        private void OnDisable()
        {
            Camera.onPostRender -= OnRendered;
        }

        private void OnRendered(Camera camera)
        {
            //dont render if this camera isnt the main camera
            if (!camera.CompareTag("MainCamera")) return;

            Material.SetPass(0);

            GL.PushMatrix();
            GL.Begin(GL.LINES);

            //draw elements
            bool alt = (Time.time * 3) % 1 > 0.5f;
            for (int e = 0; e < queue.Length; e++)
            {
                if (!queue[e].active) continue;

                //set to inactive
                queue[e].active = false;

                List<Vector3> points = new List<Vector3>();
                if (queue[e].dashed)
                {
                    //subdivide
                    const float Interval = 2f;
                    for (int i = 0; i < queue[e].points.Length; i++)
                    {
                        Vector3 pointA = queue[e].points[i];
                        Vector3 pointB = queue[e].points[(i + 1) % queue[e].points.Length];
                        Vector3 direction = pointB - pointA;
                        float magnitude = direction.magnitude;
                        int amount = Mathf.RoundToInt(magnitude / Interval);
                        direction /= magnitude;

                        for (int p = 0; p < amount - 1; ++p)
                        {
                            if (p % 2 == (alt ? 0 : 1))
                            {
                                float lerp = p / ((float)amount - 1);
                                Vector3 start = Vector3.Lerp(pointA, pointB, lerp);
                                Vector3 end = start + (direction * Interval);
                                points.Add(start);
                                points.Add(end);
                            }
                        }
                    }
                }
                else
                {
                    points.AddRange(queue[e].points);
                }

                GL.Color(queue[e].color);

                for (int i = 0; i < points.Count; i++)
                {
                    GL.Vertex(points[i]);
                }
            }
            
            GL.End();
            GL.PopMatrix();
        }
    }
}