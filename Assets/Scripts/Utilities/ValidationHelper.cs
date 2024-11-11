using System.Text;
using UnityEngine;

public static class ValidationHelper
{
    public static void EnsureGlobalSingleInstance<TObject>() where TObject : Object
    {
        var instances = Object.FindObjectsByType<TObject>(FindObjectsSortMode.None);
        if (instances.Length <= 1)
            return;

        var errorMessage = new StringBuilder();
        errorMessage.AppendLine($"There should only be one object with a {nameof(TObject)} component in the scene");
        foreach (var instance in instances)
        {
            errorMessage.AppendLine(instance.name);
        }
        Debug.LogError(errorMessage.ToString());
    }

    public static void EnsureGlobalSingleInstance<TObject>(this TObject tObject) where TObject : Object
        => EnsureGlobalSingleInstance<TObject>();
}