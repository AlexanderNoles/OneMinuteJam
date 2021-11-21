#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SaveAndLoadUtility
{

    private DialogueGraphView _targetGV;
    private NodeContainer _cachedContainer;

    private List<Edge> Edges => _targetGV.edges.ToList();

    public static SaveAndLoadUtility GetInstance(DialogueGraphView target)
    {
        return new SaveAndLoadUtility
        {
            _targetGV = target,
        };
    }

    public void Save()
    {
        if (!Edges.Any())
        {
            return;
        }
        _cachedContainer = ScriptableObject.CreateInstance<NodeContainer>();

        var allEdges = Edges.Where(x => x.input.node != null).ToArray();
        for(int i = 0; i < allEdges.Length; i++)
        {
            _cachedContainer.edges.Add(new NodeLinkData
            {
                BaseNodeGUID = allEdges[i].output.node.name,
                PortNumber = allEdges[i].output.name,
                TargetNodeGUID = allEdges[i].input.node.name,
                TargetNumber = allEdges[i].input.name
            });
        }

        foreach (GraphElement element in _targetGV.graphElements)
        {
            if(element.title == "Dialogue")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Dialogue",
                    DialogueText = (element as DialogueNode).Dialogue
                });
            }
            else if(element.title == "Choice")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Choice",
                    options = (element as ChoicesNode).options
                });
            }
            else if (element.title == "Pause")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Pause",
                    time = (element as PauseNode).time
                });
            }
            else if (element.title == "Comparison")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Comparison",
                    values = (element as ComparisonNode).values.Values.ToList()
                });
            }
            else if (element.title == "Custom")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Custom",
                    message = (element as CustomMessageNode).message
                });
            }
            else if (element.title == "Start")
            {
                _cachedContainer.nodes.Add(new NodeData
                {
                    GUID = element.name,
                    position = element.GetPosition(),
                    nodeType = "Start"
                });
            }
        }

        AssetDatabase.CreateAsset(_cachedContainer, "Assets/Resources/conversation.asset");
        AssetDatabase.SaveAssets();
    }

    public void Load()
    {
        _cachedContainer = Resources.Load<NodeContainer>("conversation");
        CreateNodes();
        ConnectNodes();
    }

    private void CreateNodes()
    {
        foreach(NodeData nodeData in _cachedContainer.nodes)
        {
            _targetGV.CreateNode(nodeData,nodeData.nodeType);
        }
    }

    private void ConnectNodes()
    {
        foreach(NodeLinkData nodeLink in _cachedContainer.edges)
        {
            Node outputNode = (Node)_targetGV.graphElements.Where(x => x.name == nodeLink.BaseNodeGUID).First();
            Port outputPort = outputNode.outputContainer.Children().Where(x => x is Port && x.name == nodeLink.PortNumber).First() as Port;

            Node inputNode = (Node)_targetGV.graphElements.Where(x => x.name == nodeLink.TargetNodeGUID).First();
            Port inputPort;
            try
            {
                inputPort = inputNode.inputContainer.Children().Where(x => x is Port && x.name == nodeLink.TargetNumber).First() as Port;
            }
            catch
            {
                inputPort = inputNode.extensionContainer.Children().Where(x => x is Port && x.name == nodeLink.TargetNumber).First() as Port;
            }

            Edge tempEdge = new Edge
            {
                output = outputPort,
                input = inputPort
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            _targetGV.Add(tempEdge);
        }
    }
}
#endif
