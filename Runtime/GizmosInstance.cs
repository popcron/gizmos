using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

#if !UNITY_2019_1_OR_NEWER
using System;

public struct ScriptableRenderContext {}

public static class RenderPipelineManager
{
    public static event Action<ScriptableRenderContext, Camera> endCameraRendering;
}

#endif

namespace Popcron
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class GizmosInstance : MonoBehaviour
    {
        private const int DefaultQueueSize = 512;

        private static GizmosInstance instance;
        private static bool hotReloaded = true;
        private static Material defaultMaterial;
        private static Plane[] cameraPlanes = new Plane[6];

        private Material overrideMaterial;
        private int queueIndex = 0;
        private Element[] queue = new Element[DefaultQueueSize];

        /// <summary>
        /// The material being used to render
        /// </summary>
        public static Material Material
        {
            get
            {
                GizmosInstance inst = GetOrCreate();
                if (inst.overrideMaterial)
                {
                    return inst.overrideMaterial;
                }

                return DefaultMaterial;
            }
            set
            {
                GizmosInstance inst = GetOrCreate();
                inst.overrideMaterial = value;
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
                    defaultMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    defaultMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                    defaultMaterial.SetInt("_Cull", (int)CullMode.Off);
                    defaultMaterial.SetInt("_ZWrite", 0);
                }

                return defaultMaterial;
            }
        }

        internal static GizmosInstance GetOrCreate()
        {
            if (hotReloaded || !instance)
            {
                bool markDirty = false;
                GizmosInstance[] gizmosInstances = FindObjectsOfType<GizmosInstance>();
                for (int i = 0; i < gizmosInstances.Length; i++)
                {
                    instance = gizmosInstances[i];

                    //destroy any extra gizmo instances
                    if (i > 0)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(gizmosInstances[i]);
                        }
                        else
                        {
                            DestroyImmediate(gizmosInstances[i]);
                            markDirty = true;
                        }
                    }
                }

                //none were found, create a new one
                if (!instance)
                {
                    instance = new GameObject("Popcron.Gizmos.GizmosInstance").AddComponent<GizmosInstance>();
                    instance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

                    markDirty = true;
                }

#if UNITY_EDITOR
                //mark scene as dirty
                if (markDirty && !Application.isPlaying)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    EditorSceneManager.MarkSceneDirty(scene);
                }
#endif

                hotReloaded = false;
            }

            return instance;
        }

        internal static void Add(Vector3[] points, Color? color, bool dashed)
        {
            GizmosInstance inst = GetOrCreate();

            //excedeed the length, so loopback
            if (inst.queueIndex >= DefaultQueueSize)
            {
                inst.queueIndex = 0;
            }

            inst.queue[inst.queueIndex].active = true;
            inst.queue[inst.queueIndex].color = color ?? Color.white;
            inst.queue[inst.queueIndex].points = points;
            inst.queue[inst.queueIndex].dashed = dashed;

            inst.queueIndex++;
        }

        private void OnEnable()
        {
            //populate queue with empty elements
            queue = new Element[DefaultQueueSize];
            for (int i = 0; i < DefaultQueueSize; i++)
            {
                queue[i] = new Element();
            }

            if (GraphicsSettings.renderPipelineAsset == null)
            {
                Camera.onPostRender += OnRendered;
            }
            else
            {
                RenderPipelineManager.endCameraRendering += OnRendered;
            }
        }

        private void OnDisable()
        {
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                Camera.onPostRender -= OnRendered;
            }
            else
            {
                RenderPipelineManager.endCameraRendering -= OnRendered;
            }
        }

        private void OnRendered(ScriptableRenderContext context, Camera camera)
        {
            OnRendered(camera);
        }

        private bool ShouldRenderCamera(Camera camera)
        {
            if (!camera)
            {
                return false;
            }

            //allow the scene and main camera always
            if (camera.name == "SceneCamera" || camera.CompareTag("MainCamera"))
            {
                return true;
            }

            //it passed through the filter
            if (Gizmos.CameraFilter?.Invoke(camera) == true)
            {
                return true;
            }

            return false;
        }

        private bool IsVisibleByCamera(Element points, Camera camera)
        {
            if (!camera)
            {
                return false;
            }

            //essentially check if at least 1 point is visible by the camera
            for (int i = 0; i < points.points.Length; i++)
            {
                Vector3 vp = camera.WorldToViewportPoint(points.points[i], camera.stereoActiveEye);
                if (vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnRendered(Camera camera)
        {
            if (!ShouldRenderCamera(camera))
            {
                return;
            }

            Vector3 offset = Gizmos.Offset;
            Material.SetPass(0);

            GL.PushMatrix();
            GL.Begin(GL.LINES);

            //draw elements
            float time = 0f;
            if (Application.isPlaying)
            {
                time = Time.time;
            }
            else
            {
#if UNITY_EDITOR
                time = (float)UnityEditor.EditorApplication.timeSinceStartup;
#endif
            }

            bool alt = time % 1 > 0.5f;
            float dashGap = Mathf.Clamp(Gizmos.DashGap, 0.01f, 32f);
            List<Vector3> points = new List<Vector3>();
            for (int e = 0; e < queue.Length; e++)
            {
                Element element = queue[e];
                if (!element.active)
                {
                    continue;
                }
                element.active = false;

                //dont render this thingy if its not inside the frustum
                if (Gizmos.FrustumCulling)
                {
                    if (!IsVisibleByCamera(element, camera))
                    {
                        continue;
                    }
                }

                points.Clear();
                if (element.dashed)
                {
                    //subdivide
                    for (int i = 0; i < element.points.Length - 1; i++)
                    {
                        Vector3 pointA = element.points[i];
                        Vector3 pointB = element.points[i + 1];
                        Vector3 direction = pointB - pointA;
                        if (direction.sqrMagnitude > dashGap * dashGap * 2f)
                        {
                            float magnitude = direction.magnitude;
                            int amount = Mathf.RoundToInt(magnitude / dashGap);
                            direction /= magnitude;

                            for (int p = 0; p < amount - 1; p++)
                            {
                                if (p % 2 == (alt ? 1 : 0))
                                {
                                    float startLerp = p / (amount - 1f);
                                    float endLerp = (p + 1) / (amount - 1f);
                                    Vector3 start = Vector3.Lerp(pointA, pointB, startLerp);
                                    Vector3 end = Vector3.Lerp(pointA, pointB, endLerp);
                                    points.Add(start);
                                    points.Add(end);
                                }
                            }
                        }
                        else
                        {
                            points.Add(pointA);
                            points.Add(pointB);
                        }
                    }
                }
                else
                {
                    points.AddRange(element.points);
                }

                GL.Color(element.color);
                for (int i = 0; i < points.Count; i++)
                {
                    GL.Vertex(points[i] + offset);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
