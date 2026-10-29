using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class PrefabCheckUtility
{
    public static bool IsSceneInstance(Component component)
    {
        if (component == null || component.gameObject == null) 
            return false;

        GameObject go = component.gameObject;

        // 1. Must belong to a valid, loaded scene (rules out Project assets & temporary import clones)
        if (!go.scene.IsValid() || !go.scene.isLoaded)
            return false;

#if UNITY_EDITOR
        // 2. Explicitly rule out Prefab Assets on disk
        if (PrefabUtility.IsPartOfPrefabAsset(component))
            return false;

        // 3. Rule out the isolated Prefab Mode editing stage (if you only want real gameplay scenes)
        if (PrefabStageUtility.GetPrefabStage(go) != null)
            return false;

        // 4. Rule out persistent objects stored on disk
        if (EditorUtility.IsPersistent(component))
            return false;
#endif

        return true;
    }
}