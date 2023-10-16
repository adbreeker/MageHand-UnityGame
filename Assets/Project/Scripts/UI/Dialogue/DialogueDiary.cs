using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDiary : MonoBehaviour
{
    //List of dialogues that the player has completed (speaker name, content)
    //public List<List<string>> dialogueDiary = new List<List<string>>();

    public Dictionary<string, List<List<string>>> dialogueDiary = new Dictionary<string, List<List<string>>>();
}