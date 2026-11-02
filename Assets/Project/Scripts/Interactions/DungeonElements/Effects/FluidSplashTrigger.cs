using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FluidSplashTrigger : MonoBehaviour
{
    public enum SplashPreset
    {
        SingleImpulse,          // 1 clean ring
        MainWaveWithRipples,    // 1 main wave + 2 smaller trailing ripples
        Custom                  // Manual slider control
    }

    private Material _waterMaterial;
    private float _lastSplashTime = -999f;


    [Header("Splash Style Preset")]
    [Tooltip("Choose a ready-made wave style or select Custom to tweak sliders manually.")]
    public SplashPreset splashPreset = SplashPreset.SingleImpulse;

    [Header("Splash Wave Customization")]
    [Tooltip("Number of wave crests generated per splash (1 = single impulse).")]
    [Range(1f, 8f)]
    public float numberOfRings = 1.0f;

    [Tooltip("Width of a single wave ring in world units.")]
    [Min(0.05f)]
    public float singleRingWidth = 0.6f;

    [Tooltip("How strongly trailing rings fade behind the leading ring.")]
    [Range(0f, 5f)]
    public float trailingRingsFalloff = 1.5f;

    [Tooltip("Expansion speed of the splash ring.")]
    [Min(0.1f)]
    public float waveSpeed = 3.5f;

    [Tooltip("How fast the wave fades out over time.")]
    [Min(0.1f)]
    public float timeDecay = 1.2f;

    [Header("Impact & Velocity Settings")]
    [Tooltip("Base wave height generated upon object impact.")]
    public float baseAmplitude = 0.15f;

    [Tooltip("Minimum time in seconds between consecutive splash triggers.")]
    public float splashCooldown = 0.15f;

    [Tooltip("Scale the wave amplitude based on the falling object's velocity.")]
    public bool scaleWithVelocity = true;

    [Tooltip("Velocity multiplier when scaling wave amplitude.")]
    public float velocityMultiplier = 0.2f;

    [Tooltip("Minimum and maximum multiplier clamp for impact velocity.")]
    public Vector2 impactClamp = new Vector2(0.4f, 2.5f);

    private static readonly int HitPositionID = Shader.PropertyToID("_HitPosition");
    private static readonly int HitTimeID = Shader.PropertyToID("_HitTime");
    private static readonly int WaveAmplitudeID = Shader.PropertyToID("_WaveAmplitude");
    private static readonly int RingCountID = Shader.PropertyToID("_RingCount");
    private static readonly int RingThicknessID = Shader.PropertyToID("_RingThickness");
    private static readonly int TrailingFalloffID = Shader.PropertyToID("_TrailingFalloff");
    private static readonly int WaveSpeedID = Shader.PropertyToID("_WaveSpeed");
    private static readonly int TimeDecayID = Shader.PropertyToID("_TimeDecay");

    private void Awake()
    {
        _waterMaterial = GetComponent<Renderer>().material;
        ApplyPresetValues();
        SyncWaveSettingsToMaterial();
    }

    private void OnValidate()
    {
        ApplyPresetValues();

        if (_waterMaterial != null)
        {
            SyncWaveSettingsToMaterial();
        }
    }

    private void ApplyPresetValues()
    {
        switch (splashPreset)
        {
            case SplashPreset.SingleImpulse:
                numberOfRings = 1.0f;
                singleRingWidth = 0.6f;
                trailingRingsFalloff = 0.0f;
                break;

            case SplashPreset.MainWaveWithRipples:
                numberOfRings = 3.0f;
                singleRingWidth = 0.6f;
                trailingRingsFalloff = 2.0f;
                break;

            case SplashPreset.Custom:
                // Keep manual slider values
                break;
        }
    }

    private void SyncWaveSettingsToMaterial()
    {
        _waterMaterial.SetFloat(RingCountID, numberOfRings);
        _waterMaterial.SetFloat(RingThicknessID, singleRingWidth);
        _waterMaterial.SetFloat(TrailingFalloffID, trailingRingsFalloff);
        _waterMaterial.SetFloat(WaveSpeedID, waveSpeed);
        _waterMaterial.SetFloat(TimeDecayID, timeDecay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - _lastSplashTime < splashCooldown) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        float impactScale = 1f;
        if (scaleWithVelocity && other.attachedRigidbody != null)
        {
            float speed = other.attachedRigidbody.linearVelocity.magnitude;
            impactScale = Mathf.Clamp(speed * velocityMultiplier, impactClamp.x, impactClamp.y);
        }

        TriggerSplash(hitPoint, baseAmplitude * impactScale);
    }

    public void TriggerSplash(Vector3 worldHitPoint, float amplitude)
    {
        _lastSplashTime = Time.time;

        ApplyPresetValues();
        SyncWaveSettingsToMaterial();

        _waterMaterial.SetVector(HitPositionID, worldHitPoint);
        _waterMaterial.SetFloat(HitTimeID, Time.time);
        _waterMaterial.SetFloat(WaveAmplitudeID, amplitude);
    }

    public List<GameObject> GetSubmergedItems()
    {
        List<GameObject> submergedItems = new List<GameObject>();
        int itemLayer = LayerMask.NameToLayer("Item");

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        Bounds searchBounds = meshCollider.bounds;
        searchBounds.Encapsulate(boxCollider.bounds);
        Collider[] candidates = Physics.OverlapBox(
            searchBounds.center,
            searchBounds.extents,
            Quaternion.identity,
            1 << itemLayer,
            QueryTriggerInteraction.Collide);

        HashSet<GameObject> uniqueItems = new HashSet<GameObject>();
        Collider[] waterColliders = { meshCollider, boxCollider };
        foreach (Collider candidate in candidates)
        {
            if (candidate.transform.parent != null)
            {
                continue;
            }

            foreach (Collider waterCollider in waterColliders)
            {
                MeshCollider waterMeshCollider = waterCollider as MeshCollider;
                MeshCollider candidateMeshCollider = candidate as MeshCollider;
                if (waterMeshCollider != null && candidateMeshCollider != null &&
                    !waterMeshCollider.convex && !candidateMeshCollider.convex)
                {
                    continue;
                }

                if (Physics.ComputePenetration(
                    waterCollider,
                    waterCollider.transform.position,
                    waterCollider.transform.rotation,
                    candidate,
                    candidate.transform.position,
                    candidate.transform.rotation,
                    out _,
                    out _))
                {
                    uniqueItems.Add(candidate.gameObject);
                    break;
                }
            }
        }

        submergedItems.AddRange(uniqueItems);
        return submergedItems;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(FluidSplashTrigger))]
public class WaterSplashTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1. Display the attached script field at the top
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        GUI.enabled = true;

        EditorGUILayout.Space();

        // 2. Preset section
        EditorGUILayout.PropertyField(serializedObject.FindProperty("splashPreset"));

        SerializedProperty splashPresetProperty = serializedObject.FindProperty(nameof(FluidSplashTrigger.splashPreset));

        EditorGUILayout.Space();

        // 3. Show wave sliders only in Custom mode
        if (splashPresetProperty.enumValueIndex == (int)FluidSplashTrigger.SplashPreset.Custom)
        {
            EditorGUILayout.LabelField("Custom Wave Parameters", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfRings"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("singleRingWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trailingRingsFalloff"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeDecay"));
            EditorGUILayout.Space();
        }

        // 4. Remaining settings (always visible)
        EditorGUILayout.LabelField("Impact Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAmplitude"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("splashCooldown"));

        SerializedProperty scaleWithVelocityProperty = serializedObject.FindProperty(nameof(FluidSplashTrigger.scaleWithVelocity));
        EditorGUILayout.PropertyField(scaleWithVelocityProperty);

        if (scaleWithVelocityProperty.boolValue)
        {
            EditorGUILayout.LabelField("Velocity Scaling", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("velocityMultiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("impactClamp"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif