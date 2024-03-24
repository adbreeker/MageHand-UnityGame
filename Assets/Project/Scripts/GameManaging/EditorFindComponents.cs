using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class EditorFindComponents : MonoBehaviour
{
    public UnityEngine.Object[] foundedComponents;

    public string typeToFind = "";
    public void FindAllOfType()
    {
        if(typeToFind != "") 
        {
            Type componentType = Type.GetType(typeToFind);
            if(componentType == null) { componentType = Type.GetType("UnityEngine." + typeToFind + ", UnityEngine"); }

            if(componentType != null ) 
            {
                foundedComponents = FindObjectsOfType(componentType);
            }
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