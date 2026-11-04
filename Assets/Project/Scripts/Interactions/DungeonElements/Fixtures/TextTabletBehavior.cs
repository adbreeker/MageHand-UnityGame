using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TextTabletBehavior : MonoBehaviour
{
    [TextArea]
    public string tabletText = "";
    public Color textColor = Color.white;

    public bool receiveLight = true;

    [SerializeField] private TextMeshPro textObject;

    private MeshRenderer textMeshRenderer;
    private MaterialPropertyBlock propBlock;

    private static readonly int UseLightingID = Shader.PropertyToID("_UseLighting");
    private static readonly int ReceiveShadowsID = Shader.PropertyToID("_ReceiveShadows");

    private void OnEnable()
    {
        ApplyTabletState();
    }

    private void Update()
    {
        if (textObject == null) return;

        // Only update when values change to save performance
        if (textObject.text != tabletText)
            textObject.text = tabletText;

        if (textObject.color != textColor)
            textObject.color = textColor;
    }

    public void SetTextLighting(bool isLit, bool receiveShadows)
    {
        if (textObject == null) return;

        if (textMeshRenderer == null)
            textMeshRenderer = textObject.GetComponent<MeshRenderer>();

        if (textMeshRenderer == null) return;

        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();

        // 1. Read the current block on the child TextMeshPro's MeshRenderer
        textMeshRenderer.GetPropertyBlock(propBlock);

        // 2. Set the shader values for THIS tablet only (zero material instances created)
        propBlock.SetFloat(UseLightingID, isLit ? 1f : 0f);
        propBlock.SetFloat(ReceiveShadowsID, receiveShadows ? 1f : 0f);

        // 3. Apply back to the MeshRenderer
        textMeshRenderer.SetPropertyBlock(propBlock);
    }

    private void OnValidate()
    {
        ApplyTabletState();
    }

    private void ApplyTabletState()
    {
        if (textObject == null) return;

        textObject.text = tabletText;
        textObject.color = textColor;
        SetTextLighting(receiveLight, receiveLight);
    }
}