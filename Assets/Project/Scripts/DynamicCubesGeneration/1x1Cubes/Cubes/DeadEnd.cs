using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DeadEnd : MonoBehaviour
{
    public WallsList.Wall selectedWall_1, selectedWall_2, selectedWall_3;

    public GameObject wall_1, wall_2, wall_3;

    Quaternion rotWall1 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, -90f, 0f);
    Quaternion rotWall3 = Quaternion.Euler(0f, 0f, 0f);

    bool showFields = true;

    private void OnValidate()
    {
        if (showFields)
        {
            selectedWall_1 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_1);
            selectedWall_2 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_2);
            selectedWall_3 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_3);

            WallsList walls = gameObject.GetComponentInParent<WallsList>();

            GameObject temp1 = Instantiate(walls.walls[(int)selectedWall_1], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            temp1.transform.localRotation = rotWall1;
            GameObject temp2 = Instantiate(walls.walls[(int)selectedWall_2], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            temp2.transform.localRotation = rotWall2;
            GameObject temp3 = Instantiate(walls.walls[(int)selectedWall_3], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
            temp3.transform.localRotation = rotWall3;

            StartCoroutine(DestroyWall(wall_1));
            StartCoroutine(DestroyWall(wall_2));
            StartCoroutine(DestroyWall(wall_3));

            wall_1 = temp1;
            wall_2 = temp2;
            wall_3 = temp3;
        }
    }

    IEnumerator DestroyWall(GameObject wall)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(wall);
    }

    [CustomEditor(typeof(DeadEnd))]
    public class DeadEndEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DeadEnd script = (DeadEnd)target;

            EditorGUILayout.Space();

            if (script.showFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_1"), new GUIContent("Select Wall 1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_2"), new GUIContent("Select Wall 2"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_3"), new GUIContent("Select Wall 3"));
            }

            if (GUILayout.Button(script.showFields ? "Lock" : "Show Fields"))
            {
                DestroyImmediate(script);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
