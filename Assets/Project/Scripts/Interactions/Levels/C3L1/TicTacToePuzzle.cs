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
        Debug.Log("Player: " + index / 3 + " " + index % 3);

        if (CheckWin(playerSymbol)) { Debug.Log("Player win"); }
        else { TicTacToeAiMove(); }
    }

    public void TicTacToeAiMove()
    {
        Vector2Int bestMove = FindBestMove();
        if (bestMove != new Vector2Int(-1, -1))
        {
            board[bestMove.x, bestMove.y] = aiSymbol;
            Debug.Log("Ai: " + bestMove.x + " " + bestMove.y);
            _gameTiles[bestMove.x * 3 + bestMove.y].MarkTile(_redMarkPrefab);
        }

        if(CheckWin(aiSymbol)) { Debug.Log("Ai win"); }
    }

    private Vector2Int FindBestMove()
    {
        // Próbuj wygrać
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == '\0') // Jeśli pole jest wolne
                {
                    board[i, j] = aiSymbol;
                    if (CheckWin(aiSymbol))
                    {
                        board[i, j] = '\0'; // Cofnij symulację
                        return new Vector2Int(i, j);
                    }
                    board[i, j] = '\0'; // Cofnij symulację
                }
            }
        }

        // Próbuj zablokować gracza
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == '\0') // Jeśli pole jest wolne
                {
                    board[i, j] = playerSymbol;
                    if (CheckWin(playerSymbol))
                    {
                        board[i, j] = '\0'; // Cofnij symulację
                        return new Vector2Int(i, j);
                    }
                    board[i, j] = '\0'; // Cofnij symulację
                }
            }
        }

        // Wykonaj losowy ruch
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

        return new Vector2Int(-1, -1); // Jeśli nie ma ruchów
    }

    private bool CheckWin(char symbol)
    {
        // Sprawdź wiersze i kolumny
        for (int i = 0; i < 3; i++)
        {
            if ((board[i, 0] == symbol && board[i, 1] == symbol && board[i, 2] == symbol) ||
                (board[0, i] == symbol && board[1, i] == symbol && board[2, i] == symbol))
            {
                return true;
            }
        }

        // Sprawdź przekątne
        if ((board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol) ||
            (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol))
        {
            return true;
        }

        return false;
    }

}