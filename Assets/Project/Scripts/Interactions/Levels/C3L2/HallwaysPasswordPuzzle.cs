using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwaysPasswordPuzzle : MonoBehaviour
{
    [SerializeField] string _password = "AETHER";
    string _currentInput = "";
    string _textToDisplay = "";

    [SerializeField] TextTabletBehavior _textTablet;

    [Header("Phone Buttons:")]
    [SerializeField] ButtonBehavior _buttonRemove;
    [SerializeField] ButtonBehavior _buttonABC;
    [SerializeField] ButtonBehavior _buttonDEF;
    [SerializeField] ButtonBehavior _buttonGHI;
    [SerializeField] ButtonBehavior _buttonJKL;
    [SerializeField] ButtonBehavior _buttonMNO;
    [SerializeField] ButtonBehavior _buttonPQRS;
    [SerializeField] ButtonBehavior _buttonTUV;
    [SerializeField] ButtonBehavior _buttonWXYZ;

    [SerializeField] float _sameButtonDelay = 1.5f;
    (Coroutine routine, string characters)_sameButtonTimer = (null, "");

    [Header("Password matched")]
    [SerializeField] List<OpenBarsPassage> _passages = new();
    [SerializeField] Color _matchedTextColor;



    public void Start()
    {
        _buttonRemove.OnButtonClickedEvents += OnButtonRemoveClicked;
        _buttonABC.OnButtonClickedEvents += OnButtonABCClicked;
        _buttonDEF.OnButtonClickedEvents += OnButtonDEFClicked;
        _buttonGHI.OnButtonClickedEvents += OnButtonGHIClicked;
        _buttonJKL.OnButtonClickedEvents += OnButtonJKLClicked;
        _buttonMNO.OnButtonClickedEvents += OnButtonMNOClicked;
        _buttonPQRS.OnButtonClickedEvents += OnButtonPQRSClicked;
        _buttonTUV.OnButtonClickedEvents += OnButtonTUVClicked;
        _buttonWXYZ.OnButtonClickedEvents += OnButtonWXYZClicked;

        foreach(char c in _password)
        {
            _textToDisplay += "_ ";
        }
        _currentInput = _currentInput.Trim();
        _textTablet.tabletText = _textToDisplay;
    }

    void PasswordMatched()
    {
        _buttonRemove.isInteractable = false;
        _buttonABC.isInteractable = false;
        _buttonDEF.isInteractable = false;
        _buttonGHI.isInteractable = false;
        _buttonJKL.isInteractable = false;
        _buttonMNO.isInteractable = false;
        _buttonPQRS.isInteractable = false;
        _buttonTUV.isInteractable = false;
        _buttonWXYZ.isInteractable = false;

        foreach (OpenBarsPassage passage in _passages)
        {
            passage.Interaction();
        }

        _textTablet.textColor = _matchedTextColor;
    }

    void UpdateTextToDisplay()
    {
        _textToDisplay = "";
        foreach (char c in _currentInput)
        {
            _textToDisplay += c + " ";
        }
        foreach (char c in _password.Substring(_currentInput.Length))
        {
            _textToDisplay += "_ ";
        }
        _textToDisplay = _textToDisplay.Trim();
        _textTablet.tabletText = _textToDisplay;

        if(_currentInput == _password)
        {
            PasswordMatched();
        }
    }

    void OnButtonRemoveClicked(GameObject button)
    {
        if(_currentInput.Length > 0)
        {
            _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            UpdateTextToDisplay();
        }

        if(_sameButtonTimer.routine != null)
        {
            StopCoroutine(_sameButtonTimer.routine);
            _sameButtonTimer.routine = null;
            _sameButtonTimer.characters = "";
        }
    }

    void OnButtonABCClicked(GameObject button)
    {
        OnKeypadButtonClicked("ABC");
    }

    void OnButtonDEFClicked(GameObject button)
    {
        OnKeypadButtonClicked("DEF");
    }

    void OnButtonGHIClicked(GameObject button)
    {
        OnKeypadButtonClicked("GHI");
    }

    void OnButtonJKLClicked(GameObject button)
    {
        OnKeypadButtonClicked("JKL");
    }

    void OnButtonMNOClicked(GameObject button)
    {
        OnKeypadButtonClicked("MNO");
    }

    void OnButtonPQRSClicked(GameObject button)
    {
        OnKeypadButtonClicked("PQRS");
    }

    void OnButtonTUVClicked(GameObject button)
    {
        OnKeypadButtonClicked("TUV");
    }

    void OnButtonWXYZClicked(GameObject button)
    {
        OnKeypadButtonClicked("WXYZ");
    }

    public void OnKeypadButtonClicked(string buttonChars)
    {
        bool isSameButtonActive = _sameButtonTimer.routine != null
                                  && _currentInput.Length > 0
                                  && buttonChars[..^1].Contains(_currentInput[^1]);

        if (isSameButtonActive)
        {
            // Same button pressed within delay -> upgrade letter
            int currentIndex = buttonChars.IndexOf(_currentInput[^1]);
            char nextChar = buttonChars[currentIndex + 1];
            _currentInput = _currentInput[..^1] + nextChar;

            UpdateTextToDisplay();
            RestartTimer(buttonChars);
        }
        else if (_currentInput.Length < _password.Length)
        {
            // Different button OR timer expired -> add first letter of this button
            _currentInput += buttonChars[0];

            UpdateTextToDisplay();
            RestartTimer(buttonChars);
        }
    }

    private void RestartTimer(string characters)
    {
        if (_sameButtonTimer.routine != null)
        {
            StopCoroutine(_sameButtonTimer.routine);
        }
        _sameButtonTimer.routine = StartCoroutine(SameButtonTimer(characters));
    }

    private IEnumerator SameButtonTimer(string characters)
    {
        _sameButtonTimer.characters = characters;
        yield return new WaitForSeconds(_sameButtonDelay);
        _sameButtonTimer.routine = null;
        _sameButtonTimer.characters = "";
    }
}
