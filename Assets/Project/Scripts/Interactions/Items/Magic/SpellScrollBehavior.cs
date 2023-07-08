using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SpellScrollInfo //class for holding spell scroll data
{
    public string spellName;
    public int manaCost;
    public string spellDescription;
    public Texture spellPicture;
}

public class SpellScrollBehavior : MonoBehaviour
{
    public SpellScrollInfo spellScrollInfo;

    private Spellbook spellbook;

    private void Start() //make scroll unpickable if spellbook wasn't picked up yet
    {
        spellbook = PlayerParams.Controllers.spellbook;

        if(!spellbook.bookOwned)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void OnPickUp() //on pick up add scroll info to spellbook and then destroy this scroll object
    {
        if(GetSpellScrollInfo() != null)
        {
            spellbook.AddSpell(GetSpellScrollInfo());
            Destroy(gameObject);
        }
    }

    public SpellScrollInfo GetSpellScrollInfo() //return spell scroll info
    {
        return spellScrollInfo;
    }
}


//custom editor ------------------------------------------------------------------------------------------ custom editor
[CustomEditor(typeof(SpellScrollBehavior))]
class SpellScrollBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpellScrollBehavior script = (SpellScrollBehavior)target;

        //custom style for labels
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontSize = 20;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        //custom styles for text areas
        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
        textAreaStyle.wordWrap = true;

        GUILayout.Space(8f);

        //make space for spell name
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Spell Name", labelStyle);
        GUILayout.Space(4f);
        script.spellScrollInfo.spellName = EditorGUILayout.TextField(script.spellScrollInfo.spellName);
        EditorGUILayout.EndVertical();

        //display rest only if spell name is written
        if(script.spellScrollInfo.spellName != "")
        {
            GUILayout.Space(8f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Mana Cost", labelStyle);
            GUILayout.Space(4f);
            script.spellScrollInfo.manaCost = EditorGUILayout.IntField(script.spellScrollInfo.manaCost);
            EditorGUILayout.EndVertical();

            GUILayout.Space(8f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Spell Description", labelStyle);
            GUILayout.Space(4f);
            script.spellScrollInfo.spellDescription = EditorGUILayout.TextArea(script.spellScrollInfo.spellDescription, textAreaStyle, GUILayout.Height(60));
            EditorGUILayout.EndVertical();

            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Spell Image", labelStyle);
            GUILayout.Space(4f);
            script.spellScrollInfo.spellPicture = (Texture)EditorGUILayout.ObjectField(script.spellScrollInfo.spellPicture, typeof(Texture), false, GUILayout.Height(300));
            EditorGUILayout.EndVertical();

            GUILayout.Space(20f);
        }

        serializedObject.ApplyModifiedProperties();
    }
}


