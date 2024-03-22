using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class Room1x1 : MonoBehaviour
{
    //selecting what wall to use:
    public WallsList1.Wall selectedWall_1, selectedWall_2, selectedWall_3, selectedWall_4;

    //wall instances in cube
    public GameObject wall_1, wall_2, wall_3, wall_4;

    //rotations of every wall position
    Quaternion rotWall1 = Quaternion.Euler(0f, 0f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall3 = Quaternion.Euler(0f, 180f, 0f);
    Quaternion rotWall4 = Quaternion.Euler(0f, 270f, 0f);

    private void OnValidate() //changing walls dynamically in editor
    {
        if (PrefabUtility.IsPartOfPrefabAsset(this) == false) //works only on scene, not as asset
        {
            //getting walls from custom editor
            selectedWall_1 = (WallsList1.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_1);
            selectedWall_2 = (WallsList1.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_2);
            selectedWall_3 = (WallsList1.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_3);
            selectedWall_4 = (WallsList1.Wall)EditorGUILayout.EnumPopup("Select a wall:", selectedWall_4);

            WallsList1 walls = gameObject.GetComponentInParent<WallsList1>();

            GameObject temp1 = null, temp2 = null, temp3 = null, temp4 = null;

            //creating new walls
            if ((int)selectedWall_1 > 0)
            {
                temp1 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_1], gameObject.transform);
                temp1.transform.localRotation = rotWall1;
            }
            if ((int)selectedWall_2 > 0)
            {
                temp2 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_2], gameObject.transform);
                temp2.transform.localRotation = rotWall2;
            }
            if ((int)selectedWall_3 > 0)
            {
                temp3 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_3], gameObject.transform);
                temp3.transform.localRotation = rotWall3;
            }
            if ((int)selectedWall_4 > 0)
            {
                temp4 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_4], gameObject.transform);
                temp4.transform.localRotation = rotWall4;
            }

            //destroying previous walls
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

    IEnumerator DestroyWall(GameObject wall) //quite strange solution so paradox problem
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(wall);
    }


    //custom editor ----------------------------------------------------------------------------------------------------------------- custom editor
    [CustomEditor(typeof(Room1x1))]
    public class Room1x1Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            Room1x1 script = (Room1x1)target;

            EditorGUILayout.Space();

            if (script != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_1"), new GUIContent("Select Wall 1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_2"), new GUIContent("Select Wall 2"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_3"), new GUIContent("Select Wall 3"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedWall_4"), new GUIContent("Select Wall 4"));
            }

            if (GUILayout.Button("Lock")) //deleting script to prevent any more changes in walls
            {
                DestroyImmediate(script);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
