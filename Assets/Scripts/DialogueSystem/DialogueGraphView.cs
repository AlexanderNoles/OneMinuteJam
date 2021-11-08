using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    private readonly NodeData defaultNode = new NodeData();

    public DialogueGraphView()
    {
        //Navigation Setup
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public override void BuildContextualMenu(UnityEngine.UIElements.ContextualMenuPopulateEvent evt)
    {
        if (evt.target is GraphView || evt.target is Node)
        {
            evt.menu.AppendAction("Create Dialogue Node", (x) => { CreateNode(defaultNode); });
            evt.menu.AppendAction("Create Choices Node", (x) => { CreateNode(defaultNode, "Choice"); });
            evt.menu.AppendAction("Create Pause Node", (x) => { CreateNode(defaultNode, "Pause"); });
        }
        base.BuildContextualMenu(evt);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)  //Defines what ports can be connected with each port
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)   //Port cannot connect to itself
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }



    public void CreateNode(NodeData nodeData, string nodeType = "Dialogue")
    {
        if(nodeType == "Dialogue")
        {
            AddElement(CreateDialogueNode(nodeData));
        }
        else if(nodeType == "Choice")
        {
            AddElement(CreateChoicesNode(nodeData));
        }
        else if (nodeType == "Pause")
        {
            AddElement(CreatePauseNode(nodeData));
        }
        else if (nodeType == "Comparison")
        {
            AddElement(CreateComparisonNode(nodeData));
        }
    }

    private DialogueNode CreateDialogueNode(NodeData nodeData)
    {
        var newNode = new DialogueNode
        {
            title = "Dialogue",
            GUID = nodeData.Guid,
            Dialogue = nodeData.DialogueText
        };

        //Dialogue Entry Field
        var dialogueField = new TextField();
        dialogueField.RegisterValueChangedCallback(evt =>
        {
            newNode.Dialogue = evt.newValue;
        });
        dialogueField.value = nodeData.DialogueText;
        newNode.contentContainer.Add(dialogueField);

        //Output/Input Port
        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.portName = "Output";
        newNode.outputContainer.Add(outputPort);

        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        newNode.RefreshExpandedState();
        newNode.RefreshPorts();
        return newNode;
    }

    private ChoicesNode CreateChoicesNode(NodeData nodeData)
    {
        var newNode = new ChoicesNode
        {
            title = "Choice",
            GUID = nodeData.Guid,
            options = nodeData.options
        };

        if(newNode.options.Count == 0)
        {
            for(int i = 0; i < 3; i++)
            {
                newNode.options.Add(string.Empty);
            }          
        }

        //Default input and output
        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.portName = "Default Output";
        newNode.outputContainer.Add(outputPort);

        //Three input field each with an output node
        CreateChoice(0,nodeData,newNode);
        CreateChoice(1,nodeData,newNode);
        CreateChoice(2,nodeData,newNode);

        return newNode;
    }

    private void CreateChoice(int index, NodeData nodeData, ChoicesNode node)
    {
        //Text Field
        var textField = new TextField();
        textField.RegisterValueChangedCallback(evt => 
        {
            node.options[index] = evt.newValue;
        });
        try
        {
            textField.value = nodeData.options[index];
        }
        catch {}           
        node.inputContainer.Add(textField);

        //Port
        var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.portName = "Output " + (index + 1);
        node.outputContainer.Add(outputPort);
    }

    private PauseNode CreatePauseNode(NodeData nodeData)
    {
        var newNode = new PauseNode
        {
            GUID = nodeData.Guid,
            title = "Pause",
            time = nodeData.time
        };

        //Time Field
        var timeField = new FloatField();
        timeField.RegisterValueChangedCallback(evt => 
        {
            newNode.time = evt.newValue;
        });
        timeField.value = nodeData.time;
        newNode.mainContainer.Add(timeField);

        //Input/Output port
        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.portName = "Output";
        newNode.outputContainer.Add(outputPort);

        return newNode;
    }

    private ComparisonNode CreateComparisonNode(NodeData nodeData)
    {
        var newNode = new ComparisonNode
        {

        };

        return newNode;
    }
}

public class DialogueNode : Node
{
    public string GUID;
    public string Dialogue;
}

public class ChoicesNode : Node
{
    public string GUID;
    public List<string> options = new List<string>();
}

public class PauseNode : Node
{
    public string GUID;
    public float time;
}

public class ComparisonNode : Node
{
    public string GUID;
}
