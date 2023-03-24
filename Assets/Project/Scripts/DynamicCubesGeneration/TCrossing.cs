using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TCrossing : MonoBehaviour
{
    public WallsList.Wall selectedWall_1;

    public GameObject wall_1;

    Quaternion rotWall1 = Quaternion.Euler(0f, 0f, 0f);

    bool showFields = true;

    private void OnValidate()
    {
        if (showFields)
        {
            selectedWall_1 = (WallsList.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_1);
        }

        WallsList walls = gameObject.GetComponentInParent<WallsList>();

        GameObject temp1 = Instantiate(walls.walls[(int)selectedWall_1], gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        temp1.transform.localRotation = rotWall1;

        StartCoroutine(DestroyWall(wall_1));

        wall_1 = temp1;
    }

    IEnumerator DestroyWall(GameObject wall)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(wall);
    }

    [CustomEditor(typeof(TCrossing))]
    public class TCrossingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TCrossing script = (TCrossing)target;

            EditorGUILayout.Space();

            if (script.showFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_1"), new GUIContent("Select Wall 1"));
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
