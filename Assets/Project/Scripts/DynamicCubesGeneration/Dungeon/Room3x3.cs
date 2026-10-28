using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class Room3x3 : MonoBehaviour
{
    //selecting what wall to use:
    public WallsList1.Wall selectedWall_1_1, selectedWall_1_2, selectedWall_1_3, 
        selectedWall_2_1, selectedWall_2_2, selectedWall_2_3, 
        selectedWall_3_1, selectedWall_3_2, selectedWall_3_3,
        selectedWall_4_1, selectedWall_4_2, selectedWall_4_3;

    //wall instances in cube
    public GameObject wall_1_1, wall_1_2, wall_1_3, 
        wall_2_1, wall_2_2, wall_2_3, 
        wall_3_1, wall_3_2, wall_3_3, 
        wall_4_1, wall_4_2, wall_4_3;

    //rotations of every wall position
    Quaternion rotWall1 = Quaternion.Euler(0f, 0f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall3 = Quaternion.Euler(0f, 180f, 0f);
    Quaternion rotWall4 = Quaternion.Euler(0f, 270f, 0f);

    bool isRebuildScheduled;

    private void Start()
    {
        Debug.LogWarning("Room3x3 script is still present on the GameObject. Consider removing it to prevent further changes.");
    }

    private void OnValidate() //changing walls dynamically in editor
    {
        if (PrefabUtility.IsPartOfPrefabAsset(this))
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

        if (this == null || gameObject == null || PrefabUtility.IsPartOfPrefabAsset(this))
        {
            return;
        }

        WallsList1 walls = gameObject.GetComponentInParent<WallsList1>();
        if (walls == null || walls.walls == null)
        {
            return;
        }

        GameObject temp1_1 = null, temp1_2 = null, temp1_3 = null, 
            temp2_1 = null, temp2_2 = null, temp2_3 = null, 
            temp3_1 = null, temp3_2 = null, temp3_3 = null, 
            temp4_1 = null, temp4_2 = null, temp4_3 = null;

        if ((int)selectedWall_1_1 > 0)
        {
            temp1_1 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_1_1], gameObject.transform);
            temp1_1.transform.localRotation = rotWall1;
            temp1_1.transform.localPosition = new Vector3(4f, 0f, -4f);
        }
        if ((int)selectedWall_1_2 > 0)
        {
            temp1_2 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_1_2], gameObject.transform);
            temp1_2.transform.localRotation = rotWall1;
            temp1_2.transform.localPosition = new Vector3(0f, 0f, -4f);
        }
        if ((int)selectedWall_1_3 > 0)
        {
            temp1_3 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_1_3], gameObject.transform);
            temp1_3.transform.localRotation = rotWall1;
            temp1_3.transform.localPosition = new Vector3(-4f, 0f, -4f);
        }
        if ((int)selectedWall_2_1 > 0)
        {
            temp2_1 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_2_1], gameObject.transform);
            temp2_1.transform.localRotation = rotWall2;
            temp2_1.transform.localPosition = new Vector3(-4f, 0f, -4f);
        }
        if ((int)selectedWall_2_2 > 0)
        {
            temp2_2 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_2_2], gameObject.transform);
            temp2_2.transform.localRotation = rotWall2;
            temp2_2.transform.localPosition = new Vector3(-4f, 0f, 0f);
        }
        if ((int)selectedWall_2_3 > 0)
        {
            temp2_3 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_2_3], gameObject.transform);
            temp2_3.transform.localRotation = rotWall2;
            temp2_3.transform.localPosition = new Vector3(-4f, 0f, 4f);
        }
        if ((int)selectedWall_3_1 > 0)
        {
            temp3_1 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_3_1], gameObject.transform);
            temp3_1.transform.localRotation = rotWall3;
            temp3_1.transform.localPosition = new Vector3(-4f, 0f, 4f);
        }
        if ((int)selectedWall_3_2 > 0)
        {
            temp3_2 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_3_2], gameObject.transform);
            temp3_2.transform.localRotation = rotWall3;
            temp3_2.transform.localPosition = new Vector3(0f, 0f, 4f);
        }
        if ((int)selectedWall_3_3 > 0)
        {
            temp3_3 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_3_3], gameObject.transform);
            temp3_3.transform.localRotation = rotWall3;
            temp3_3.transform.localPosition = new Vector3(4f, 0f, 4f);
        }
        if ((int)selectedWall_4_1 > 0)
        {
            temp4_1 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_4_1], gameObject.transform);
            temp4_1.transform.localRotation = rotWall4;
            temp4_1.transform.localPosition = new Vector3(4f, 0f, 4f);
        }
        if ((int)selectedWall_4_2 > 0)
        {
            temp4_2 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_4_2], gameObject.transform);
            temp4_2.transform.localRotation = rotWall4;
            temp4_2.transform.localPosition = new Vector3(4f, 0f, 0f);
        }
        if ((int)selectedWall_4_3 > 0)
        {
            temp4_3 = (GameObject)PrefabUtility.InstantiatePrefab(walls.walls[(int)selectedWall_4_3], gameObject.transform);
            temp4_3.transform.localRotation = rotWall4;
            temp4_3.transform.localPosition = new Vector3(4f, 0f, -4f);
        }

        DestroyImmediate(wall_1_1);
        DestroyImmediate(wall_1_2);
        DestroyImmediate(wall_1_3);
        DestroyImmediate(wall_2_1);
        DestroyImmediate(wall_2_2);
        DestroyImmediate(wall_2_3);
        DestroyImmediate(wall_3_1);
        DestroyImmediate(wall_3_2);
        DestroyImmediate(wall_3_3);
        DestroyImmediate(wall_4_1);
        DestroyImmediate(wall_4_2);
        DestroyImmediate(wall_4_3);

        wall_1_1 = temp1_1;
        wall_1_2 = temp1_2;
        wall_1_3 = temp1_3;
        wall_2_1 = temp2_1;
        wall_2_2 = temp2_2;
        wall_2_3 = temp2_3;
        wall_3_1 = temp3_1;
        wall_3_2 = temp3_2;
        wall_3_3 = temp3_3;
        wall_4_1 = temp4_1;
        wall_4_2 = temp4_2;
        wall_4_3 = temp4_3;
    }


    //custom editor ----------------------------------------------------------------------------------------------------------------- custom editor
    [CustomEditor(typeof(Room3x3))]
    public class Room3x3Editor : Editor
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
            Room3x3 script = (Room3x3)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(script), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (script != null)
            {
                EditorGUILayout.LabelField("Wall 1", EditorStyles.boldLabel);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_1_1"), "Wall 1_1 (Green)", Color.green);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_1_2"), "Wall 1_2 (Green)", Color.green);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_1_3"), "Wall 1_3 (Green)", Color.green);
                EditorGUILayout.LabelField("Wall 2", EditorStyles.boldLabel);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_2_1"), "Wall 2_1 (Yellow)", Color.yellow);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_2_2"), "Wall 2_2 (Yellow)", Color.yellow);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_2_3"), "Wall 2_3 (Yellow)", Color.yellow);
                EditorGUILayout.LabelField("Wall 3", EditorStyles.boldLabel);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_3_1"), "Wall 3_1 (Red)", Color.red);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_3_2"), "Wall 3_2 (Red)", Color.red);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_3_3"), "Wall 3_3 (Red)", Color.red);
                EditorGUILayout.LabelField("Wall 4", EditorStyles.boldLabel);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_4_1"), "Wall 4_1 (Blue)", Color.cyan);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_4_2"), "Wall 4_2 (Blue)", Color.cyan);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_4_3"), "Wall 4_3 (Blue)", Color.cyan);
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
        if (PrefabUtility.IsPartOfPrefabAsset(this))
        {
            return;
        }

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);

        style.normal.textColor = Color.green;
        Handles.Label(transform.position - transform.forward * 5.5f + transform.right * 4f, "1", style);
        Handles.Label(transform.position - transform.forward * 5.5f, "2", style);
        Handles.Label(transform.position - transform.forward * 5.5f - transform.right * 4f, "3", style);

        style.normal.textColor = Color.yellow;
        Handles.Label(transform.position - transform.right * 5.5f - transform.forward * 4f, "1", style);
        Handles.Label(transform.position - transform.right * 5.5f, "2", style);
        Handles.Label(transform.position - transform.right * 5.5f + transform.forward * 4f, "3", style);

        style.normal.textColor = Color.red;
        Handles.Label(transform.position + transform.forward * 5.5f - transform.right * 4f, "1", style);
        Handles.Label(transform.position + transform.forward * 5.5f, "3", style);
        Handles.Label(transform.position + transform.forward * 5.5f + transform.right * 4f, "3", style);

        style.normal.textColor = Color.cyan;
        Handles.Label(transform.position + transform.right * 5.5f + transform.forward * 4f, "1", style);
        Handles.Label(transform.position + transform.right * 5.5f, "4", style);
        Handles.Label(transform.position + transform.right * 5.5f - transform.forward * 4f, "3", style);
    }
}
#endif
