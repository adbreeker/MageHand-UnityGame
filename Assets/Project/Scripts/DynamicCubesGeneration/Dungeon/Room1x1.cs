using System.Collections;
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

    bool isRebuildScheduled;

    private void Start()
    {
        Debug.LogWarning("Room1x1 script is still present on the GameObject. Consider removing it to prevent further changes.");
    }

    private void OnValidate() //changing walls dynamically in editor
    {
        if (!PrefabCheckUtility.IsSceneInstance(this))
        {
            return;
        }

        ScheduleRebuild();
    }

    private void ScheduleRebuild()
    {
        if (isRebuildScheduled)
        {
            return;
        }

        isRebuildScheduled = true;
        EditorApplication.delayCall += RebuildWalls;
    }

    private void RebuildWalls()
    {
        isRebuildScheduled = false;

        if (!PrefabCheckUtility.IsSceneInstance(this))
        {
            return;
        }

        WallsList1 walls = gameObject.GetComponentInParent<WallsList1>();
        if (walls == null || walls.walls == null)
        {
            return;
        }

        GameObject temp1 = null, temp2 = null, temp3 = null, temp4 = null;

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

        DestroyImmediate(wall_1);
        DestroyImmediate(wall_2);
        DestroyImmediate(wall_3);
        DestroyImmediate(wall_4);

        wall_1 = temp1;
        wall_2 = temp2;
        wall_3 = temp3;
        wall_4 = temp4;
    }


    //custom editor ----------------------------------------------------------------------------------------------------------------- custom editor
    [CustomEditor(typeof(Room1x1))]
    public class Room1x1Editor : Editor
    {
        private static void DrawColoredWallProperty(SerializedProperty property, string label, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = color;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, labelStyle, GUILayout.Width(EditorGUIUtility.labelWidth - 4f));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            Room1x1 script = (Room1x1)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(script), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (script != null)
            {
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_1"), "Wall 1 (Green)", Color.green);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_2"), "Wall 2 (Yellow)", Color.yellow);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_3"), "Wall 3 (Red)", Color.red);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_4"), "Wall 4 (Blue)", Color.cyan);
            }

            EditorGUILayout.Space(8f);
            if (GUILayout.Button("Refresh")) //refreshing walls
            {
                script.ScheduleRebuild();
            }

            EditorGUILayout.Space(16f);
            if (GUILayout.Button("Lock", GUILayout.Height(50f))) //deleting script to prevent any more changes in walls
            {
                DestroyImmediate(script);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OnDrawGizmos()
    {
        if (!PrefabCheckUtility.IsSceneInstance(this))
        {
            return;
        }

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);

        style.normal.textColor = Color.green;
        Handles.Label(transform.position - transform.forward * 1.5f, "1", style);

        style.normal.textColor = Color.yellow;
        Handles.Label(transform.position - transform.right * 1.5f, "2", style);

        style.normal.textColor = Color.red;
        Handles.Label(transform.position + transform.forward * 1.5f, "3", style);

        style.normal.textColor = Color.cyan;
        Handles.Label(transform.position + transform.right * 1.5f, "4", style);
    }
}
#endif
