using System;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron.Gizmos
{
    public abstract class Drawer
    {
        private static Dictionary<Type, Drawer> typeToDrawer = null;

        public abstract Vector3[] Draw(params object[] args);

        protected Drawer()
        {

        }

        public static Drawer Get<T>() where T : class
        {
            //find all drawers
            if (typeToDrawer == null)
            {
                typeToDrawer = new Dictionary<Type, Drawer>();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsAbstract) continue;
                        if (type.IsSubclassOf(typeof(Drawer)))
                        {
                            Drawer value = (Drawer)Activator.CreateInstance(type);
                            typeToDrawer.Add(type, value);
                        }
                    }
                }
            }

            if (typeToDrawer.TryGetValue(typeof(T), out Drawer drawer))
            {
                return drawer;
            }
            else
            {
                return null;
            }
        }
    }
}
