using System;
using System.Collections.Generic;
using UnityEngine;


public class OpenDialogue : MonoBehaviour
{
    [Header("Activate dialogue")]
    public bool allowToActivate = true;

    [Header("Game objects")]
    public Canvas dialogueCanvas;
    [Header("Parameters")]
    public float textSpeed = 0.02f;
    //Should not be long
    public bool saveDialogue = true;
    public bool showTree = false;
    [Tooltip("Shouldn't be long")]
    public string dialogueSaveName;

    public event Action DialogueStarted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerParams.Objects.player && allowToActivate)
        {
            allowToActivate = false;

            DialogueStarted?.Invoke();

            if (saveDialogue)
            {
                if (!PlayerParams.Controllers.journal.dialoguesJournal.ContainsKey(dialogueSaveName))
                {
                    PlayerParams.Controllers.journal.dialoguesJournal.Add(dialogueSaveName, new List<List<string>>());
                }
            }

            dialogueCanvas.gameObject.SetActive(true);
        }
    }

    private List<DialogueTreeElement> dialogueTree = new List<DialogueTreeElement>();
    private List<int> treeEndpoints = new List<int>();
    private class DialogueTreeElement 
    {
        public Dialogue selfDialogue;
        public List<Dialogue> chlidren;
    }
    /*
    public class TreeEndpoint
    {
        public Dialogue selfDialogue;
        public int points;
    }
    */

    private void Start()
    {
        CreateDialogueTree(dialogueCanvas.GetComponent<Dialogue>(), 0);
        int max = 0;
        int min = 1000000000;
        foreach(int endpoint in treeEndpoints)
        {
            if (endpoint < min) min = endpoint;
            if (endpoint > max) max = endpoint;
        }

        PlayerParams.Controllers.pointsManager.maxPlotPoints += max;
        PlayerParams.Controllers.pointsManager.minPlotPoints += min;


        if (showTree)
        {
            string text = "||||||||Dialogue tree of " + dialogueSaveName + "||||||||\n\nTree elements count: " + dialogueTree.Count + "\n";
            foreach (DialogueTreeElement dialogueTreeElement in dialogueTree)
            {
                text += "[[" + dialogueTreeElement.selfDialogue.name + "]]: ";
                foreach (Dialogue child in dialogueTreeElement.chlidren)
                {
                    text += child.gameObject.name + " | ";
                }
                text = text.Remove(text.Length-2) + "\n";
            }
            text += "\nTree endpoints cound: " + treeEndpoints.Count + "\n";

            foreach (int endpoint in treeEndpoints)
            {
                text = text + endpoint + " ";
            }
            Debug.Log(text + "\n");
        }    
    }

    private void CreateDialogueTree(Dialogue dialogue, int points)
    {
        //assign values for better workflow
        List<Dialogue> nextOptions = new List<Dialogue>();
        Dictionary<int, Dialogue> optionsChoices = new Dictionary<int, Dialogue>();
        Dictionary<int, string> optionsTexts = new Dictionary<int, string>();
        Dictionary<int, int> optionsPoints = new Dictionary<int, int>();
        //1
        if (dialogue.option1Choice != null)
        {
            nextOptions.Add(dialogue.option1Choice.GetComponent<Dialogue>());
            optionsChoices.Add(1, dialogue.option1Choice.GetComponent<Dialogue>());
        }
        optionsTexts.Add(1, dialogue.option1Text);
        optionsPoints.Add(1, dialogue.option1Points);

        //2
        if (dialogue.option2Choice != null)
        {
            nextOptions.Add(dialogue.option2Choice.GetComponent<Dialogue>());
            optionsChoices.Add(2, dialogue.option2Choice.GetComponent<Dialogue>());
        }
        optionsTexts.Add(2, dialogue.option2Text);
        optionsPoints.Add(2, dialogue.option2Points);
        //3
        if (dialogue.option3Choice != null)
        {
            nextOptions.Add(dialogue.option3Choice.GetComponent<Dialogue>());
            optionsChoices.Add(3, dialogue.option3Choice.GetComponent<Dialogue>());
        }
        optionsTexts.Add(3, dialogue.option3Text);
        optionsPoints.Add(3, dialogue.option3Points);
        //4
        if (dialogue.option4Choice != null)
        {
            nextOptions.Add(dialogue.option4Choice.GetComponent<Dialogue>());
            optionsChoices.Add(4, dialogue.option4Choice.GetComponent<Dialogue>());
        }
        optionsTexts.Add(4, dialogue.option4Text);
        optionsPoints.Add(4, dialogue.option4Points);

        //create new element
        DialogueTreeElement dialogueTreeElement = new DialogueTreeElement();
        dialogueTreeElement.selfDialogue = dialogue;
        dialogueTreeElement.chlidren = nextOptions;
        dialogueTree.Add(dialogueTreeElement);


        //activate next iterrations or create endpoints
        for (int i = 1; i<5; i++)
        {
            if (optionsChoices.ContainsKey(i) && !string.IsNullOrWhiteSpace(optionsTexts[i]))
            {
                CreateDialogueTree(optionsChoices[i], points + optionsPoints[i]);
            }
            else if (!optionsChoices.ContainsKey(i) && optionsTexts.ContainsKey(i) && !string.IsNullOrWhiteSpace(optionsTexts[i]))
            {
                /*
                TreeEndpoint treeEndpoint = new TreeEndpoint();
                treeEndpoint.selfDialogue = dialogue;
                treeEndpoint.points = points + optionsPoints[i];
                */
                treeEndpoints.Add(points + optionsPoints[i]);
            }
        }
    }
}