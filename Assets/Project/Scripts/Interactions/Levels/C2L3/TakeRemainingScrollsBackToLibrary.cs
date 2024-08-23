using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeRemainingScrollsBackToLibrary : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] OpenDialogue _specialDialogue;

    [Header("Possition to restart dialogue")]
    [SerializeField] Transform _takingTriggerCube;

    [Header("Possible tp transforms")]
    [SerializeField] List<Transform> _tpTransforms;

    bool _takingTriggered = false;
    bool _dialogueWasntActivated = true;

    private void Update()
    { 
        if(PlayerParams.Controllers.playerMovement.currentTile == _takingTriggerCube && !_takingTriggered)
        {
            if (IsAnyReadableInPossession())
            {
                if(_dialogueWasntActivated)
                {
                    _specialDialogue.allowToActivate = true;
                    _dialogueWasntActivated = false;
                }
                _takingTriggered = true;
                TeleportRemainingScrolls();
            }
        }

        if(PlayerParams.Controllers.playerMovement.currentTile != _takingTriggerCube && _takingTriggered) { _takingTriggered= false; } 
    }

    bool IsAnyReadableInPossession()
    {
        foreach(GameObject item in PlayerParams.Controllers.inventory.inventory)
        {
            if(item.GetComponent<ReadableBehavior>() != null)
            {
                return true;
            }
        }

        if(PlayerParams.Controllers.handInteractions.inHand != null && PlayerParams.Controllers.handInteractions.inHand.GetComponent<ReadableBehavior>() != null)
        {
            return true;
        }

        return false;
    }

    void TeleportRemainingScrolls()
    {
        int tpIndex = 0;

        if (PlayerParams.Controllers.handInteractions.inHand != null && PlayerParams.Controllers.handInteractions.inHand.GetComponent<ReadableBehavior>() != null)
        {
            GameObject item = PlayerParams.Controllers.handInteractions.inHand;

            item.transform.SetParent(null);
            PlayerParams.Controllers.handInteractions.inHand = null;

            AudioSource tpSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation);
            tpSound.Play();
            Destroy(tpSound, tpSound.clip.length);

            GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);
            Instantiate(tpEffect, transform.position, Quaternion.identity);

            item.layer = LayerMask.NameToLayer("Item");

            item.GetComponent<ItemBehavior>().TeleportTo(_tpTransforms[tpIndex].transform.position, 0f, null);
            item.transform.rotation = _tpTransforms[tpIndex].transform.rotation;

            tpIndex = (tpIndex + 1) % _tpTransforms.Count;
        }

        for (int i = PlayerParams.Controllers.inventory.inventory.Count - 1; i >= 0; i--)
        {
            GameObject item = PlayerParams.Controllers.inventory.inventory[i];
            if (item.GetComponent<ReadableBehavior>() != null)
            {
                PlayerParams.Controllers.inventory.inventory.RemoveAt(i);
                item.SetActive(true);

                item.transform.SetParent(null);

                AudioSource tpSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_MagicalTeleportation);
                tpSound.Play();
                Destroy(tpSound, tpSound.clip.length);

                GameObject tpEffect = GameParams.Holders.materialsAndEffectsHolder.GetEffect(MaterialsAndEffectsHolder.Effects.teleportationObject);
                Instantiate(tpEffect, transform.position, Quaternion.identity);

                item.layer = LayerMask.NameToLayer("Item");

                item.GetComponent<ItemBehavior>().TeleportTo(_tpTransforms[tpIndex].transform.position, 0f, null);
                item.transform.rotation = _tpTransforms[tpIndex].transform.rotation;

                tpIndex = (tpIndex + 1) % _tpTransforms.Count;
            }
        }
    }
}
