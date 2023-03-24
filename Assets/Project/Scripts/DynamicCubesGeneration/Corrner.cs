using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class Corrner : MonoBehaviour
{
    public WallsList.Wall selectedWall_1, selectedWall_2;

    public GameObject wall_1, wall_2;

    Quaternion rotWall1 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, 0f, 0f);

    bool showFields = true;

    private void OnValidate()
    {
        if (showFields)
        {
            selectedWall_1 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_1);
            selectedWall_2 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_2);
        }

        WallsList walls = gameObject.GetComponentInParent<WallsList>();

        GameObject temp1 = Instantiate(walls.walls[(int)selectedWall_1], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        temp1.transform.localRotation = rotWall1;
        GameObject temp2 = Instantiate(walls.walls[(int)selectedWall_2], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        temp2.transform.localRotation = rotWall2;

        StartCoroutine(DestroyWall(wall_1));
        StartCoroutine(DestroyWall(wall_2));

        wall_1 = temp1;
        wall_2 = temp2;
    }

    IEnumerator DestroyWall(GameObject wall)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(wall);
    }

    [CustomEditor(typeof(Corrner))]
    public class CorrnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Corrner script = (Corrner)target;

            EditorGUILayout.Space();

            if (script.showFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_1"), new GUIContent("Select Wall 1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_2"), new GUIContent("Select Wall 2"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("showFields"), new GUIContent("Show Fields"));
            }

            if (GUILayout.Button(script.showFields ? "Lock" : "Unlock Fields"))
            {
                script.showFields = !script.showFields;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
