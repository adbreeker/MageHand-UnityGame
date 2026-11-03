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
    [SerializeField] LineRenderer _winningLineRenderer;
    [SerializeField] GameObject _dispelEffectPrefab;
    [SerializeField] Vector3 _failTeleportDestination;

    private void Start()
    {
        StartCoroutine(StartListener());
        foreach(TicTacToeTile tile in _gameTiles)
        {
            tile.OnPlayerMarkedTile += ParseTicTacToePlayerMove;
        }
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

        yield return new WaitForSeconds(0.5f);
        TicTacToeAiMove();
    }

    private char[,] board = new char[3, 3]; // 'X', 'O', or '\0'
    private char aiSymbol = 'O';           
    private char playerSymbol = 'X';       

    void ParseTicTacToePlayerMove(TicTacToeTile markedTile)
    {
        int index = _gameTiles.IndexOf(markedTile);
        board[index / 3, index % 3] = playerSymbol;

        if (CheckWin(playerSymbol))
        {
            StartCoroutine(DrawWinningLine(playerSymbol));
            StartCoroutine(PlayerSuccesDeleyed(1.5f));
        }
        else { TicTacToeAiMove(); }
    }

    public void TicTacToeAiMove()
    {
        Vector2Int bestMove = FindBestMove();
        if (bestMove != new Vector2Int(-1, -1))
        {
            board[bestMove.x, bestMove.y] = aiSymbol;
            _gameTiles[bestMove.x * 3 + bestMove.y].MarkTile(_redMarkPrefab);
        }

        if(CheckWin(aiSymbol)) 
        {
            StartCoroutine(DrawWinningLine(aiSymbol));
            StartCoroutine(PlayerFailDeleyed(1.5f));
        }
        else if(CheckDraw())
        {
            RemoveAllMarks();
            StartCoroutine(PlayerSuccesDeleyed(0.5f));
        }
    }

    private Vector2Int FindBestMove()
    {
        // try to win
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == '\0') // if field is clear
                {
                    board[i, j] = aiSymbol;
                    if (CheckWin(aiSymbol))
                    {
                        board[i, j] = '\0'; // revert simulation
                        return new Vector2Int(i, j);
                    }
                    board[i, j] = '\0'; // revert simulation
                }
            }
        }

        // try to block player
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == '\0') // if field is clear
                {
                    board[i, j] = playerSymbol;
                    if (CheckWin(playerSymbol))
                    {
                        board[i, j] = '\0'; // revert simulation
                        return new Vector2Int(i, j);
                    }
                    board[i, j] = '\0'; // revert simulation
                }
            }
        }

        // semi random move
        List<Vector2Int> randoms = new() { new Vector2Int(0, 0), new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,0), new Vector2Int(2,2) };
        while(randoms.Count > 0)
        {
            Vector2Int index = randoms[Random.Range(0, randoms.Count)];
            if (board[index.x, index.y] == '\0')
            {
                return index;
            }
            else
            {
                randoms.Remove(index);
            }
        }
        randoms = new() { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 2), new Vector2Int(2, 1) };
        while (randoms.Count > 0)
        {
            Vector2Int index = randoms[Random.Range(0, randoms.Count)];
            if (board[index.x, index.y] == '\0')
            {
                return index;
            }
            else
            {
                randoms.Remove(index);
            }
        }

        return new Vector2Int(-1, -1); // no moves
    }

    private bool CheckWin(char symbol)
    {
        // check rows and columns
        for (int i = 0; i < 3; i++)
        {
            if ((board[i, 0] == symbol && board[i, 1] == symbol && board[i, 2] == symbol) ||
                (board[0, i] == symbol && board[1, i] == symbol && board[2, i] == symbol))
            {
                return true;
            }
        }

        // check diagonals
        if ((board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol) ||
            (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol))
        {
            return true;
        }

        return false;
    }

    bool CheckDraw()
    {
        foreach (char symbol in board)
        {
            if(symbol == '\0') { return false; }
        }
        if(!CheckWin(aiSymbol) && !CheckWin(playerSymbol))
        {
            return true;
        }

        return false;
    }

    IEnumerator DrawWinningLine(char winnerSymbol)
    {
        List<Vector3> marksPositions = new List<Vector3>();

        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == winnerSymbol && board[i, 1] == winnerSymbol && board[i, 2] == winnerSymbol)
            {
                marksPositions.Add(_gameTiles[i * 3].mark.transform.position);
                marksPositions.Add(_gameTiles[i * 3 + 1].mark.transform.position);
                marksPositions.Add(_gameTiles[i * 3 + 2].mark.transform.position);
                break;
            }
            else if (board[0, i] == winnerSymbol && board[1, i] == winnerSymbol && board[2, i] == winnerSymbol)
            {
                marksPositions.Add(_gameTiles[i].mark.transform.position);
                marksPositions.Add(_gameTiles[3 + i].mark.transform.position);
                marksPositions.Add(_gameTiles[6 + i].mark.transform.position);
                break;
            }
        }

        if (marksPositions.Count == 0)
        {
            if (board[0, 0] == winnerSymbol && board[1, 1] == winnerSymbol && board[2, 2] == winnerSymbol)
            {
                marksPositions.Add(_gameTiles[0].mark.transform.position);
                marksPositions.Add(_gameTiles[4].mark.transform.position);
                marksPositions.Add(_gameTiles[8].mark.transform.position);
            }
            else if (board[0, 2] == winnerSymbol && board[1, 1] == winnerSymbol && board[2, 0] == winnerSymbol)
            {
                marksPositions.Add(_gameTiles[2].mark.transform.position);
                marksPositions.Add(_gameTiles[4].mark.transform.position);
                marksPositions.Add(_gameTiles[6].mark.transform.position);
            }
        }

        yield return new WaitForSeconds(0.5f);

        _winningLineRenderer.positionCount = marksPositions.Count;
        _winningLineRenderer.SetPositions(marksPositions.ToArray());
        if(winnerSymbol == playerSymbol) { _winningLineRenderer.startColor = Color.green; }
        else { _winningLineRenderer.startColor = Color.red; }
        _winningLineRenderer.endColor = _winningLineRenderer.startColor;
        _winningLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.5f);

        RemoveAllMarks();
    }

    void RemoveAllMarks()
    {
        _winningLineRenderer.enabled = false;
        foreach(TicTacToeTile tile in _gameTiles)
        {
            if(tile.mark != null)
            {
                Instantiate(_dispelEffectPrefab, tile.mark.transform.position, Quaternion.identity);
                Destroy(tile.mark);
                tile.tileMarked = false;
            }
        }
    }

    IEnumerator PlayerFailDeleyed(float deley)
    {
        yield return new WaitForSeconds(deley);
        PlayerParams.Controllers.playerMovement.TeleportTo(_failTeleportDestination, 0f, null);
        foreach (OpenBarsPassage bar in _bars)
        {
            bar.Interaction();
        }
        board = new char[3, 3];
        StartCoroutine(StartListener());
    }

    IEnumerator PlayerSuccesDeleyed(float deley)
    {
        foreach (TicTacToeTile tile in _gameTiles)
        {
            tile.enabled = false;
        }
        yield return new WaitForSeconds(deley);
        foreach (OpenBarsPassage bar in _bars)
        {
            bar.Interaction();
        }
    }
}