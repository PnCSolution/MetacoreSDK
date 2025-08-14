using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Component = UnityEngine.Component;

public static class MetacoreCaches
{
    public static Dictionary<int, Component> Components = new Dictionary<int, Component>();

    public static bool TryGetComponent<T>(int key, out T value) where T : Component
    {
        if (Components.TryGetValue(key, out var component) && component is T typeComponent)
        {
            value = typeComponent;
            return true;
        }

        value = null;
        return false;
    }

    public static void AddComponent<T>(int key, T value) where T : Component
    {
        Components[key] = value;
    }

    public static void RemoveComponent(int key)
    {
        Components.Remove(key);
    }

    public static T GetComponent<T>(int key) where T : Component
    {
        if (Components.ContainsKey(key) == false)
            return null;

        return Components[key] as T;
    }
}
