using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class NodeData
{
    public string Guid;
    public string nodeType;
    //Dialogue
    public string DialogueText;
    //Choices
    public List<string> options = new List<string>();
    //Pause
    public float time;
}
