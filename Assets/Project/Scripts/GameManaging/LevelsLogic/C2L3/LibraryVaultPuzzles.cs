using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryVaultPuzzles : MonoBehaviour
{
    [Header("First 4 scrolls:")]
    [SerializeField] ItemDetecting[] _first4Scrolls = new ItemDetecting[4];
    [SerializeField] OpenWallPassage _wallAfter4Scrolls;

    [Header("5th scroll")]
    [SerializeField] ItemDetecting _fifthScroll;
    [SerializeField] OpenWallPassage _wallAfter5thScroll;

    [Header("Fake scrolls for secret:")]
    [SerializeField] ItemDetecting[] _fakeScrolls = new ItemDetecting[3];
    [SerializeField] OpenWallPassage _wallAfterFakeScrolls;

    [Header("Special script for taking scrolls back via script")]
    [SerializeField] TakeRemainingScrollsBackToLibrary _specialScript;


    bool _check4Scrolls = true;
    bool _check5thScroll = false;
    bool _checkFakeScrolls = false;

    private void Awake()
    {
        GameParams.Managers.levelInfoManager.AddSecretOnLevel();
    }

    private void Update()
    {
        if(_check4Scrolls) { Check4Scrolls(); }
        if(_check5thScroll) {  Check5thScroll(); }
        if(_checkFakeScrolls) { CheckFakeScrolls(); }
    }

    void Check4Scrolls()
    {
        bool allScrolls = true;
        foreach(ItemDetecting itemDetecting in _first4Scrolls)
        {
            if(!itemDetecting.isItemInBoundaries)
            {
                allScrolls = false;
                break;
            }
        }

        if(allScrolls)
        {
            _wallAfter4Scrolls.Interaction();
            _check4Scrolls = false;
            _check5thScroll = true;
            foreach(ItemDetecting itemDetecting in _first4Scrolls)
            {
                itemDetecting.itemToDetect.GetComponent<ItemBehavior>().isInteractable = false;
            }
        }
    }

    void Check5thScroll()
    {
        if(_fifthScroll.isItemInBoundaries)
        {
            _wallAfter5thScroll.Interaction();
            _check5thScroll = false;
            _checkFakeScrolls = true;
            _fifthScroll.itemToDetect.GetComponent<ItemBehavior>().isInteractable = false;

            _specialScript.enabled = true;
        }
    }

    void CheckFakeScrolls() 
    {
        bool allScrolls = true;
        foreach (ItemDetecting itemDetecting in _fakeScrolls)
        {
            if (!itemDetecting.isItemInBoundaries)
            {
                allScrolls = false;
                break;
            }
        }

        if (allScrolls)
        {
            _wallAfterFakeScrolls.Interaction();
            _checkFakeScrolls = false;
            foreach (ItemDetecting itemDetecting in _fakeScrolls)
            {
                itemDetecting.itemToDetect.GetComponent<ItemBehavior>().isInteractable = false;
            }

            GameParams.Managers.levelInfoManager.SecretFoundPopUp();
        }
    }
}
