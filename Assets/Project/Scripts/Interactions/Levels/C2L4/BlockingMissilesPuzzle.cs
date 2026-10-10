using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingMissilesPuzzle : MonoBehaviour
{
    [Header("Starting lever")]
    [SerializeField] LeverBehavior _startLever;

    [Header("Cannons:")]
    [SerializeField] List<WallCannonBehavior> _cannons = new List<WallCannonBehavior>(4);
    [Header("Buttons:")]
    [SerializeField] List<ButtonBehavior> _buttons = new List<ButtonBehavior>();

    [Header("Opening")]
    [SerializeField] OpenBarsPassage _bars;
    [SerializeField] OpenWallPassage _wall;

    Coroutine _puzzleCoroutine;

    void Start()
    {
        _startLever.OnLeverPulledDownEvents += (g) => { _puzzleCoroutine = StartCoroutine(PuzzleCoroutine()); };
        foreach(ButtonBehavior button in _buttons) 
        {
            button.OnButtonClickedEvents += (g) => { if (_puzzleCoroutine != null) { StartCoroutine(ResetPuzzle()); } };
        }
    }

    IEnumerator PuzzleCoroutine()
    {
        _bars.Interaction();

        yield return new WaitForSeconds(1f);
        _cannons[1].LaunchMissile();

        yield return new WaitForSeconds(2f);
        _cannons[3].LaunchMissile();

        yield return new WaitForSeconds(2f);
        _cannons[0].LaunchMissile();

        yield return new WaitForSeconds(1f);
        _cannons[0].LaunchMissile();

        yield return new WaitForSeconds(2f);
        _cannons[2].LaunchMissile();

        yield return new WaitForSeconds(1.5f);
        _cannons[1].LaunchMissile();

        yield return new WaitForSeconds(1.5f);
        _cannons[2].LaunchMissile();

        yield return new WaitForSeconds(2f);
        _cannons[3].LaunchMissile();

        yield return new WaitForSeconds(1f);
        _cannons[3].LaunchMissile();

        yield return new WaitForSeconds(1f);
        _cannons[3].LaunchMissile();

        yield return new WaitForSeconds(1.5f);
        _cannons[1].LaunchMissile();

        yield return new WaitForSeconds(2f);
        _cannons[0].LaunchMissile();

        //succes

        yield return new WaitForSeconds(4f);

        foreach (ButtonBehavior button in _buttons)
        {
            button.OnButtonClickedEvents = null;
        }

        _wall.Interaction();
        _bars.Interaction();
        Destroy(this);
    }

    IEnumerator ResetPuzzle()
    {
        StopCoroutine(_puzzleCoroutine);
        _puzzleCoroutine = null;

        _startLever.OnClick();

        foreach(ButtonBehavior button in _buttons)
        {
            button.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        yield return new WaitForSeconds(3.5f);

        _startLever.gameObject.layer = LayerMask.NameToLayer("Interaction");

        foreach (ButtonBehavior button in _buttons)
        {
            button.gameObject.layer = LayerMask.NameToLayer("Interaction");
        }

        _bars.Interaction();
    }
}
