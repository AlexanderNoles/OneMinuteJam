using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class NodeData
{
    public string GUID = Guid.NewGuid().ToString();
    public Rect position;
    public string nodeType = "null";
    //Dialogue
    public string DialogueText;
    //Choices
    public List<string> options = new List<string>();
    //Pause
    public float time;
    //Comparison
    public List<float> values = new List<float>();
    //Custom
    public string message;
}
