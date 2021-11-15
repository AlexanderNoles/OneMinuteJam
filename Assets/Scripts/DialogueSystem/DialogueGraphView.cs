using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    private NodeData defaultNode
    {
        get { return new NodeData(); }
    }

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
            evt.menu.AppendAction("Create Start Node", (x) => { CreateNode(defaultNode, "Start"); });
            evt.menu.AppendAction("Create Dialogue Node", (x) => { CreateNode(defaultNode); });
            evt.menu.AppendAction("Create Choices Node", (x) => { CreateNode(defaultNode, "Choice"); });
            evt.menu.AppendAction("Create Pause Node", (x) => { CreateNode(defaultNode, "Pause"); });
            evt.menu.AppendAction("Create Comparison Node", (x) => { CreateNode(defaultNode, "Comparison"); });
            evt.menu.AppendAction("Create Message Node", (x) => { CreateNode(defaultNode, "Custom"); });
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
        else if(nodeType == "Custom")
        {
            AddElement(CreateCustomMessageNode(nodeData));
        }
        else if(nodeType == "Start")
        {
            AddElement(CreateStartNode(nodeData));
        }
    }

    private Node CreateStartNode(NodeData nodeData)
    {
        var newNode = new Node
        {
            title = "Start",
            name = nodeData.GUID
        };

        newNode.SetPosition(nodeData.position);

        //Output Node
        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.name = "0";
        outputPort.portName = "Output";
        newNode.outputContainer.Add(outputPort);

        return newNode;
    }

    private DialogueNode CreateDialogueNode(NodeData nodeData)
    {
        var newNode = new DialogueNode
        {
            title = "Dialogue",
            name = nodeData.GUID,
            Dialogue = nodeData.DialogueText
        };

        newNode.SetPosition(nodeData.position);

        //Dialogue Entry Field
        var dialogueField = new TextField();
        dialogueField.RegisterValueChangedCallback(evt =>
        {
            newNode.Dialogue = evt.newValue;
        });
        dialogueField.value = nodeData.DialogueText;
        newNode.contentContainer.Add(dialogueField);

        //Output/Input Port
        CreateBasicInputAndOutput(newNode);

        newNode.RefreshExpandedState();
        newNode.RefreshPorts();
        return newNode;
    }

    private CustomMessageNode CreateCustomMessageNode(NodeData nodeData)
    {
        var newNode = new CustomMessageNode
        {
            name = nodeData.GUID,
            title = "Message",

            message = nodeData.message
        };

        newNode.SetPosition(nodeData.position);

        //Message Entry Field
        var messageField = new TextField();
        messageField.RegisterValueChangedCallback(evt => 
        {
            newNode.message = evt.newValue;
        });
        messageField.value = nodeData.message;
        newNode.contentContainer.Add(messageField);

        CreateBasicInputAndOutput(newNode);

        newNode.RefreshExpandedState();
        newNode.RefreshPorts();
        return newNode;
    }

    private void CreateBasicInputAndOutput(Node newNode)
    {
        //Output/Input Port
        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.name = "0";
        outputPort.portName = "Output";
        newNode.outputContainer.Add(outputPort);

        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.name = "1";
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);
    }

    private ChoicesNode CreateChoicesNode(NodeData nodeData)
    {
        var newNode = new ChoicesNode
        {
            title = "Choice",
            name = nodeData.GUID,
            options = nodeData.options
        };

        newNode.SetPosition(nodeData.position);

        if (newNode.options.Count == 0)
        {
            for(int i = 0; i < 3; i++)
            {
                newNode.options.Add(string.Empty);
            }          
        }

        //Default input and output
        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.name = "0";
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.name = "1";
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
        outputPort.name = node.outputContainer.Children().Where(x => x is Port).Count().ToString() +1;
        outputPort.portName = "Output " + (index + 1);
        node.outputContainer.Add(outputPort);
    }

    private PauseNode CreatePauseNode(NodeData nodeData)
    {
        var newNode = new PauseNode
        {
            name = nodeData.GUID,
            title = "Pause",
            time = nodeData.time
        };

        newNode.SetPosition(nodeData.position);

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
        inputPort.name = "0";
        inputPort.portName = "Input";
        newNode.inputContainer.Add(inputPort);

        var outputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        outputPort.name = "1";
        outputPort.portName = "Output";
        newNode.outputContainer.Add(outputPort);

        return newNode;
    }

    private ComparisonNode CreateComparisonNode(NodeData nodeData)
    {
        var newNode = new ComparisonNode
        {
            name = nodeData.GUID,
            title = "Comparison",
        };

        newNode.SetPosition(nodeData.position);

        //Add new comparison button
        var button = new Button(() => { AddNewComparisonSlot(newNode); });
        button.text = "New Comparison Slot";
        newNode.titleButtonContainer.Add(button);

        //Input
        var inputPort = newNode.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        inputPort.name = "0";
        inputPort.portName = "Input";
        newNode.extensionContainer.Add(inputPort);

        //Add all existing comparisons
        foreach (float value in nodeData.values) //Run through nodeData values to create a dictionary
        {
            AddNewComparisonSlot(newNode, value);
        }
        newNode.RefreshPorts();
        newNode.RefreshExpandedState();
        return newNode;
    }

    private void AddNewComparisonSlot(ComparisonNode node, float currentValue = 0)
    {
        var newSlot = new FloatField();
        newSlot.name = Guid.NewGuid().ToString();
        node.values.Add(newSlot.name, 0f);
        newSlot.RegisterValueChangedCallback(evt =>
        {
            node.values[newSlot.name] = evt.newValue;
        });
        newSlot.value = currentValue;
        node.values[newSlot.name] = currentValue;

        var generatedPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(int)); //Type is abritary as there is no need to pass data between the nodes
        generatedPort.name = node.outputContainer.Children().Where(x => x is Port).Count().ToString() + 1;
        generatedPort.portName = "";
        node.outputContainer.Add(generatedPort);

        var deleteButton = new Button(() => RemoveComparisonSlot(node, newSlot, generatedPort))
        {
            text = "X"
        };
        newSlot.contentContainer.Add(deleteButton);
        node.inputContainer.Add(newSlot);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private void RemoveComparisonSlot(ComparisonNode node, FloatField slot, Port outputPort)
    {
        //Remove the edge
        var targetEdge = edges.ToList().Where(x => x.output.name == outputPort.name && x.output.node == node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }

        //Remove port and entry slot
        node.inputContainer.Remove(slot);
        node.outputContainer.Remove(outputPort);
        node.values.Remove(slot.name);
    }
}

public class DialogueNode : Node
{
    public string Dialogue;
}

public class ChoicesNode : Node
{
    public List<string> options = new List<string>();
}

public class PauseNode : Node
{
    public float time;
}

public class ComparisonNode : Node
{
    public Dictionary<string, float> values = new Dictionary<string, float>();
}

public class CustomMessageNode : Node 
{
    public string message;
}

