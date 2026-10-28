using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class Hallway : MonoBehaviour
{
    //selecting what wall to use:
    public WallsList1.Wall selectedWall_1, selectedWall_2;

    //wall instances in cube
    public GameObject wall_1, wall_2;

    //rotations of every wall position
    Quaternion rotWall1 = Quaternion.Euler(0f, 90f, 0f);
    Quaternion rotWall2 = Quaternion.Euler(0f, -90f, 0f);

    bool isRebuildScheduled;

    private void Start()
    {
        Debug.LogWarning("Hallway script is still present on the GameObject. Consider removing it to prevent further changes.");
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

        GameObject temp1 = null, temp2 = null;

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

        DestroyImmediate(wall_1);
        DestroyImmediate(wall_2);

        wall_1 = temp1;
        wall_2 = temp2;
    }


    //custom editor ----------------------------------------------------------------------------------------------------------------- custom editor
    [CustomEditor(typeof(Hallway))]
    public class HallwayEditor : Editor
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
            Hallway script = (Hallway)target;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(script), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (script != null)
            {
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_1"), "Wall 1 (Green)", Color.green);
                DrawColoredWallProperty(serializedObject.FindProperty("selectedWall_2"), "Wall 2 (Red)", Color.red);
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
        Handles.Label(transform.position - transform.right * 1.5f, "1", style);

        style.normal.textColor = Color.red;
        Handles.Label(transform.position + transform.right * 1.5f, "2", style);
    }
}
#endif
