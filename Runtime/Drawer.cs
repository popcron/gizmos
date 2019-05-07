using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Popcron
{
    public abstract class Drawer
    {
        private static Dictionary<Type, Drawer> typeToDrawer = null;

        public abstract int Draw(ref Vector3[] buffer, params object[] args);

        protected Drawer()
        {

        }

        public static Drawer Get<T>() where T : class
        {
            //find all drawers
            if (typeToDrawer == null)
            {
                typeToDrawer = new Dictionary<Type, Drawer>();

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
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

            Drawer drawer;
            if (typeToDrawer.TryGetValue(typeof(T), out drawer))
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
