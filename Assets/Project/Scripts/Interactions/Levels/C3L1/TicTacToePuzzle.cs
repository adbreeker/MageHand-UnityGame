using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToePuzzle : MonoBehaviour
{
    [Header("Start puzzle:")]
    [SerializeField] Transform _startTile;
    [SerializeField] List<OpenBarsPassage> _bars = new List<OpenBarsPassage>();

    [Header("Game:")]
    [SerializeField] GameObject _redMarkPrefab;
    [SerializeField] List<TicTacToeTile> _gameTiles = new List<TicTacToeTile>();

    private void Start()
    {
        StartCoroutine(StartListener());
    }

    IEnumerator StartListener()
    {
        while(PlayerParams.Controllers.playerMovement.currentTile != _startTile)
        {
            yield return null;
        }

        foreach (OpenBarsPassage bar in _bars)
        {
            bar.Interaction();
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(TicTacToeDefense());
    }

    IEnumerator TicTacToeDefense()
    {
        yield return null;
        _gameTiles[0].MarkTile(_redMarkPrefab);
    }
}