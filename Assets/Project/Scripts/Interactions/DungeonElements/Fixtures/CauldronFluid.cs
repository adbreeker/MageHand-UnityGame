using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronFluid : FluidSplashTrigger
{
    Color _baseColor;
    Renderer _renderer;

    public void Start()
    {
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
    }

    public IEnumerator SetColor_Base()
    {
        return TransitionColor(_baseColor);
    }

    public IEnumerator SetColor(Color targetColor)
    {
        return TransitionColor(targetColor);
    }

    IEnumerator TransitionColor(Color targetColor)
    {
        Color startColor = _renderer.material.color;
        float elapsed = 0f;
        const float duration = 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _renderer.material.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }

        _renderer.material.color = targetColor;
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
