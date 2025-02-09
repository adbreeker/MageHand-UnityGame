using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManyButtonWallPuzzle : MonoBehaviour
{
    [SerializeField] OpenBarsPassage _bars;
    [SerializeField] List<ButtonBehavior> _allButtons = new List<ButtonBehavior>();
    [SerializeField] List<GameObject> _buttonsCodeOrder = new List<GameObject>();
    Queue<GameObject> _currentCode = new Queue<GameObject>();


    private void Start()
    {
        foreach (ButtonBehavior button in _allButtons) 
        {
            button.OnButtonClickedEvents += AddButtonToCode;
        }
    }

    private void Update()
    {
        if(CheckCode())
        {
            _bars.Interaction();
            Destroy(this);
        }
    }

    bool CheckCode()
    {
        if(_currentCode.Count == _buttonsCodeOrder.Count) 
        {
            for(int i = 0; i< _currentCode.Count; i++)
            {
                if (_currentCode.ToArray()[i] != _buttonsCodeOrder[i])
                {
                    return false;
                }
            }
        }
        else { return false; }

        return true;
    }

    public void AddButtonToCode(GameObject button)
    {
        _currentCode.Enqueue(button);
        if(_currentCode.Count > _buttonsCodeOrder.Count) 
        {
            _currentCode.Dequeue();
        }
    }
}
