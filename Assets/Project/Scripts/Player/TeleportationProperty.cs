using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TeleportationSetup
{
    public enum HeightOption { Current, Custom }
    public enum RotationOption { Current, North, East, South, West, Custom }

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

    public Quaternion GetTpRotation(GameObject teleported)
    {
        switch (rotationType) 
        {
            case RotationOption.Current:
                return Quaternion.identity;
            case RotationOption.North:
                return Quaternion.Euler(teleported.transform.rotation.eulerAngles.x, 0f, teleported.transform.rotation.eulerAngles.z);
            case RotationOption.East:
                return Quaternion.Euler(teleported.transform.rotation.eulerAngles.x, 90f, teleported.transform.rotation.eulerAngles.z);
            case RotationOption.South:
                return Quaternion.Euler(teleported.transform.rotation.eulerAngles.x, 180f, teleported.transform.rotation.eulerAngles.z);
            case RotationOption.West:
                return Quaternion.Euler(teleported.transform.rotation.eulerAngles.x, 270f, teleported.transform.rotation.eulerAngles.z);
            case RotationOption.Custom:
                return Quaternion.Euler(teleported.transform.rotation.eulerAngles.x, customRotation, teleported.transform.rotation.eulerAngles.z);
            default:
                Debug.LogWarning("Teleportation used default rotation");
                return Quaternion.identity;
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
                dest2D.vector2Value = EditorGUI.Vector2Field(fieldPosition, new GUIContent("Destination"), dest2D.vector2Value);
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
