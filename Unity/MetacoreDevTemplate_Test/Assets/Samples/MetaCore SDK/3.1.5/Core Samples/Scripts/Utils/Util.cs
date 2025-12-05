using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Metacore
{
    public static class Util
    {
        public static GameObject FindChild(GameObject go, string name, bool recursive = true)
        {
            Transform child = FindChild<Transform>(go, name, recursive);
            if (child == null)
                return null;

            return child.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name, bool recursive = true) where T : Object
        {
            if (go == null)
            {
                return null;
            }

            if (recursive)
            {
                T[] components = go.GetComponentsInChildren<T>();
                foreach (T component in components)
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                    {
                        return component;
                    }
                }
            }
            else
            {
                foreach (Transform child in go.transform)
                {
                    if (string.IsNullOrEmpty(name) || child.name == name)
                    {
                        T component = child.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }

            return null;
        }
    }
}