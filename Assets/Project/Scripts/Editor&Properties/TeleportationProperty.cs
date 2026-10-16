using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TeleportationSetup
{
    public enum HeightOption { Current, Custom }
    public enum RotationOption
    {
        Current,
        [InspectorName("North (0f)")] North,
        [InspectorName("East (90f)")] East,
        [InspectorName("South (180f)")] South,
        [InspectorName("West (270f)")] West,
        Custom
    }

    public HeightOption heightType;

    [SerializeField] private Vector2 destination2D;
    [SerializeField] private Vector3 destination3D;

    public RotationOption rotationType;

    [SerializeField] private float customRotation;

    
    public Vector3 GetTpDestination(GameObject teleported)
    {
        switch (heightType)
        {
            case HeightOption.Current:
                return new Vector3(destination2D.x, teleported.transform.position.y, destination2D.y);
            case HeightOption.Custom:
                return destination3D;
            default:
                Debug.LogWarning("Teleportation used default destination");
                return Vector3.zero;
        }
    }

    public float GetTpRotation(GameObject teleported)
    {
        switch (rotationType) 
        {
            case RotationOption.Current:
                return Mathf.Round(teleported.transform.rotation.eulerAngles.y / 90f) * 90f;
            case RotationOption.North:
                return 0f;
            case RotationOption.East:
                return 90f;
            case RotationOption.South:
                return 180f;
            case RotationOption.West:
                return 270f;
            case RotationOption.Custom:
                return customRotation;
            default:
                Debug.LogWarning("Teleportation used default rotation");
                return 0f;
        }
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TeleportationSetup))]
public class TeleportationSetupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw foldout (correct label formatting)
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                 property.isExpanded, label, true);

        if (property.isExpanded)
        {
            // Increase indentation
            EditorGUI.indentLevel++;

            // Define rects for better spacing
            Rect fieldPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            // Height Field
            SerializedProperty heightProp = property.FindPropertyRelative("heightType");
            EditorGUI.PropertyField(fieldPosition, heightProp);
            fieldPosition.y += EditorGUIUtility.singleLineHeight + 2;

            // Destination Field (Single Field for Both Vector2 & Vector3)
            if ((TeleportationSetup.HeightOption)heightProp.enumValueIndex == TeleportationSetup.HeightOption.Current)
            {
                SerializedProperty dest2D = property.FindPropertyRelative("destination2D");
                Vector2 destination = dest2D.vector2Value;
                float[] values = { destination.x, destination.y };
                GUIContent[] axisLabels = { new GUIContent("X"), new GUIContent("Z") };

                EditorGUI.MultiFloatField(fieldPosition, new GUIContent("Destination"), axisLabels, values);
                dest2D.vector2Value = new Vector2(values[0], values[1]);
            }
            else
            {
                SerializedProperty dest3D = property.FindPropertyRelative("destination3D");
                dest3D.vector3Value = EditorGUI.Vector3Field(fieldPosition, new GUIContent("Destination"), dest3D.vector3Value);
            }
            fieldPosition.y += EditorGUIUtility.singleLineHeight + 2;

            // Rotation Field
            SerializedProperty rotationProp = property.FindPropertyRelative("rotationType");
            EditorGUI.PropertyField(fieldPosition, rotationProp);
            fieldPosition.y += EditorGUIUtility.singleLineHeight + 2;

            // Custom Rotation Field (Only if Rotation = Custom)
            if ((TeleportationSetup.RotationOption)rotationProp.enumValueIndex == TeleportationSetup.RotationOption.Custom)
            {
                SerializedProperty customRot = property.FindPropertyRelative("customRotation");
                EditorGUI.PropertyField(fieldPosition, customRot);
                fieldPosition.y += EditorGUIUtility.singleLineHeight + 2;
            }

            // Reset indentation
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight * 4 + 6;
        if (property.FindPropertyRelative("rotationType").enumValueIndex == (int)TeleportationSetup.RotationOption.Custom)
        {
            height += EditorGUIUtility.singleLineHeight + 2;
        }

        return height;
    }
}
#endif
