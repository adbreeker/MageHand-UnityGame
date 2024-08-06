using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class EditorFindComponents : MonoBehaviour
{
    public UnityEngine.Object[] foundedComponents;

    public string typeComponent = "";

    public string typeGUID = "";
    public void FindAllOfType()
    {
        if(typeComponent != "") 
        {
            Type componentType = Type.GetType(typeComponent);
            if(componentType == null) { componentType = Type.GetType("UnityEngine." + typeComponent + ", UnityEngine"); }

            if(componentType != null ) 
            {
                foundedComponents = FindObjectsOfType(componentType);
            }
        }
        if(typeGUID != "")
        {
            Debug.Log(AssetDatabase.GUIDToAssetPath(typeGUID));
        }
    }
}

[CustomEditor(typeof(EditorFindComponents))]
public class EditorFindComponentsEditor:Editor
{
    EditorFindComponents script;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        script = (EditorFindComponents)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Find All")) //deleting script to prevent any more changes in walls
        {
            script.FindAllOfType();
        }

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif