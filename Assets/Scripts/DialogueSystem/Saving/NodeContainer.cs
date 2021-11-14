using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NodeContainer : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
    public List<NodeLinkData> edges = new List<NodeLinkData>();
}
