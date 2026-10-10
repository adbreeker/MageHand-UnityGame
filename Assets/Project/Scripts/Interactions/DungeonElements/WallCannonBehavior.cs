using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WallCannonBehavior : MonoBehaviour
{
    public enum MissileType
    {
        teleportingMissile,
        freezingMissile,
        stoneMissile
    }

    [Header("Launching settings:")]
    public MissileType missileType;
    public GameObject[] missilesPrefabs;
    public float launchingDeley;
    [SerializeField] bool _isLaunching = true;

    [Header("Additional launch settings:")]

    [Header("Teleporting missile")]
    public Vector3 tpDestination;
    public float tpRotation;
    public bool tpOnCurrentRotation = true;

    [Header("Freezing missile")]
    public float freezeDuration;

    [Header("Stone missile")]
    public float stoneForce;

    Vector3 _launchingPos;

    private void Start()
    {
        _launchingPos = transform.position + transform.up * 0.5f;
        if (_isLaunching) { StartCoroutine(LaunchingCoroutine()); }
    }

    IEnumerator LaunchingCoroutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(launchingDeley);
            LaunchMissile();
        }
    }

    public void SetLaunching(bool setLaunching)
    {
        if (setLaunching)
        {
            _isLaunching = true;
            StopAllCoroutines();
            StartCoroutine(LaunchingCoroutine());
        }
        else
        {
            _isLaunching = false;
            StopAllCoroutines();
        }
    }

    public bool IsLaunching()
    {
        return _isLaunching;
    }

    public void LaunchMissile()
    {
        switch (missileType)
        {
            case MissileType.teleportingMissile:
                LaunchTeleportingMissile();
                break;
            case MissileType.freezingMissile:
                LaunchFreezingMissile();
                break;
            case MissileType.stoneMissile:
                LaunchStoneMissile();
                break;
        }
    }

    public void LaunchTeleportingMissile()
    {
        Quaternion _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        GameObject missile = Instantiate(missilesPrefabs[(int)missileType], _launchingPos, _launchingRot);

        TeleportingMissileBehavior missileBehavior = missile.GetComponent<TeleportingMissileBehavior>();
        missileBehavior.teleportationDestination = tpDestination;
        if(!tpOnCurrentRotation)
        {
            missileBehavior.teleportOnCurrentRotation = false;
            missileBehavior.teleportationRotation = tpRotation;
        }

        Rigidbody rb = missile.GetComponent<Rigidbody>();
        rb.AddForce(gameObject.transform.up * 10, ForceMode.Impulse);
    }

    public void LaunchFreezingMissile()
    {
        Quaternion _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        GameObject missile = Instantiate(missilesPrefabs[(int)missileType], _launchingPos, _launchingRot);

        FreezingMissileBehavior missileBehavior = missile.GetComponent<FreezingMissileBehavior>();
        missileBehavior.freezeDuration = freezeDuration;

        Rigidbody rb = missile.GetComponent<Rigidbody>();
        rb.AddForce(gameObject.transform.up * 10, ForceMode.Impulse);
    }

    public void LaunchStoneMissile()
    {
        Quaternion _launchingRot = transform.rotation * Quaternion.Euler(180.0f, 0, 0);
        GameObject missile = Instantiate(missilesPrefabs[(int)missileType], _launchingPos, _launchingRot);

        StoneMissileBehavior missileBehavior = missile.GetComponent<StoneMissileBehavior>();
        missileBehavior.pushForce = stoneForce;

        Rigidbody rb = missile.GetComponent<Rigidbody>();
        rb.AddForce(gameObject.transform.up * 10, ForceMode.Impulse);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WallCannonBehavior))]
public class WallCannonBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
        GUI.enabled = true;

        // Reference to the target script
        WallCannonBehavior wallCannon = (WallCannonBehavior)target;

        // Draw the default fields up to the MissileType selection
        EditorGUILayout.LabelField("Launching settings:", EditorStyles.boldLabel);

        //missile type
        wallCannon.missileType = (WallCannonBehavior.MissileType)EditorGUILayout.EnumPopup("Missile Type", wallCannon.missileType);
        
        //missiles prefabs 
        SerializedProperty missilesPrefabsProp = serializedObject.FindProperty("missilesPrefabs");
        EditorGUILayout.PropertyField(missilesPrefabsProp, new GUIContent("Missiles Prefabs"), true);

        //launching deley
        wallCannon.launchingDeley = EditorGUILayout.FloatField("Launching Delay", wallCannon.launchingDeley);

        //is launching
        SerializedProperty isLaunchingProp = serializedObject.FindProperty("_isLaunching");
        EditorGUILayout.PropertyField(isLaunchingProp, new GUIContent("Is Launching"));

        EditorGUILayout.Space();

        // Conditionally show additional settings based on the selected missile type
        EditorGUILayout.LabelField("Additional launch settings:", EditorStyles.boldLabel);

        if (wallCannon.missileType == WallCannonBehavior.MissileType.teleportingMissile)
        {
            EditorGUILayout.LabelField("Teleporting missile", EditorStyles.boldLabel);
            wallCannon.tpDestination = EditorGUILayout.Vector3Field("TP Destination", wallCannon.tpDestination);
            wallCannon.tpRotation = EditorGUILayout.FloatField("TP Rotation", wallCannon.tpRotation);
            wallCannon.tpOnCurrentRotation = EditorGUILayout.Toggle("TP On Current Rotation", wallCannon.tpOnCurrentRotation);
        }
        else if (wallCannon.missileType == WallCannonBehavior.MissileType.freezingMissile)
        {
            EditorGUILayout.LabelField("Freezing missile", EditorStyles.boldLabel);
            wallCannon.freezeDuration = EditorGUILayout.FloatField("Freeze Duration", wallCannon.freezeDuration);
        }
        else if (wallCannon.missileType == WallCannonBehavior.MissileType.stoneMissile)
        {
            EditorGUILayout.LabelField("Stone missile", EditorStyles.boldLabel);
            wallCannon.stoneForce = EditorGUILayout.FloatField("Stone Force", wallCannon.stoneForce);
        }

        // Apply any changes made in the Inspector
        if (GUI.changed)
        {
            EditorUtility.SetDirty(wallCannon);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
