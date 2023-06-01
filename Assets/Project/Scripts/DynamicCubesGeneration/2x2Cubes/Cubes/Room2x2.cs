using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class Room2x2 : MonoBehaviour
{
    public WallsList2.Wall selectedWall_1, selectedWall_2, selectedWall_3, selectedWall_4;

    public GameObject wall_1, wall_2, wall_3, wall_4;

    Quaternion rotWall1 = Quaternion.Euler(0f, 0f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall3 = Quaternion.Euler(0f, 180f, 0f);
    Quaternion rotWall4 = Quaternion.Euler(0f, 270f, 0f);

    bool showFields = true;

    private void OnValidate()
    {
        if (showFields)
        {
            selectedWall_1 = (WallsList2.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_1);
            selectedWall_2 = (WallsList2.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_2);
            selectedWall_3 = (WallsList2.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_3);
            selectedWall_4 = (WallsList2.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_4);

            WallsList2 walls = gameObject.GetComponentInParent<WallsList2>();

            GameObject temp1 = null, temp2 = null, temp3 = null, temp4 = null;

            if((int)selectedWall_1 > 0)
            {
                temp1 = Instantiate(walls.walls[(int)selectedWall_1], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                temp1.transform.localRotation = rotWall1;
            }
            if ((int)selectedWall_2 > 0)
            {
                temp2 = Instantiate(walls.walls[(int)selectedWall_2], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                temp2.transform.localRotation = rotWall2;
            }
            if ((int)selectedWall_3 > 0)
            {
                temp3 = Instantiate(walls.walls[(int)selectedWall_3], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                temp3.transform.localRotation = rotWall3;
            }
            if ((int)selectedWall_4 > 0)
            {
                temp4 = Instantiate(walls.walls[(int)selectedWall_4], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                temp4.transform.localRotation = rotWall4;
            }
  
            StartCoroutine(DestroyWall(wall_1));
            StartCoroutine(DestroyWall(wall_2));
            StartCoroutine(DestroyWall(wall_3));
            StartCoroutine(DestroyWall(wall_4));

            wall_1 = temp1;
            wall_2 = temp2;
            wall_3 = temp3;
            wall_4 = temp4;
        }
    }

    IEnumerator DestroyWall(GameObject wall)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(wall);
    }

    [CustomEditor(typeof(Room2x2))]
    public class DeadEndEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Room2x2 script = (Room2x2)target;

            EditorGUILayout.Space();

            if (script.showFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_1"), new GUIContent("Select Wall 1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_2"), new GUIContent("Select Wall 2"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_3"), new GUIContent("Select Wall 3"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_4"), new GUIContent("Select Wall 4"));
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
