using System.Collections;
using UnityEngine;

public class MirrorTest : MonoBehaviour
{
    [SerializeField] MagicMirrorBehavior _mirror;
    [SerializeField] float _switchingDeley = 5f;

    private void Start()
    {
        StartCoroutine(RepetitiveMirrorSwitching(new WaitForSeconds(_switchingDeley)));
    }

    IEnumerator RepetitiveMirrorSwitching(WaitForSeconds deley)
    {
        bool showPlayer = true;
        while (true) 
        {
            yield return deley;
            _mirror.ShowPlayerImage(showPlayer);
            showPlayer = !showPlayer;
        }
    }
}
