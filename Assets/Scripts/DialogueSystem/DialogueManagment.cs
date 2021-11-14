using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManagment : MonoBehaviour
{
    private NodeContainer _cachedContainer;
    private NodeData currentNode;

    //Dialogue
    public static bool dialogueFinished;

    //Choice
    private int chosenOption;
    private bool choiceChosen;

    //Pause
    private float startTime;
    private bool startTimeSet;

    //Comparison
    private float comparatorValue;

    //Custom
    public GameObject customCodeObject;
    public static bool customCodeRunning;
    private bool checkRunning;

    private void Awake()
    {
        _cachedContainer = Resources.Load<NodeContainer>("conversation");
        findStartNode();
    }

    private void findStartNode()
    {
        foreach(NodeData nodeData in _cachedContainer.nodes)
        {
            if(nodeData.nodeType == "Start")
            {
                currentNode = nodeData;
                break;
            }
        }
    }

    private void findNextNodeInChain()
    {
        if(currentNode.nodeType == "Choice")
        {
            if(chosenOption == 0)
            {
                foreach(NodeLinkData nodeLinkData in _cachedContainer.edges)
                {
                    if (int.Parse(nodeLinkData.PortNumber) == chosenOption + 1)
                    {
                        FindTargetNode(nodeLinkData);
                        break;
                    }
                }
            }
        }
        else if(currentNode.nodeType == "Comparison")
        {
            List<float> valuesToCompareTo = currentNode.values;
            for(int i = 0; i < valuesToCompareTo.Count; i++)
            {
                if(valuesToCompareTo[i] < comparatorValue)
                {
                    foreach (NodeLinkData nodeLinkData in _cachedContainer.edges)
                    {
                        if (int.Parse(nodeLinkData.PortNumber) == i+1)
                        {
                            FindTargetNode(nodeLinkData);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            //Node only has one output
            //Find guid for node that this node outputs to
            foreach (NodeLinkData nodeLinkData in _cachedContainer.edges)
            {
                if(nodeLinkData.BaseNodeGUID == currentNode.GUID)
                {
                    FindTargetNode(nodeLinkData);
                    break;
                }
            }
        }

        //Reset Values
        choiceChosen = false;
        checkRunning = false;
        startTimeSet = false;
    }

    private void FindTargetNode(NodeLinkData nodeLinkData)
    {
        foreach (NodeData nodeData in _cachedContainer.nodes)
        {
            if (nodeData.GUID == nodeLinkData.TargetNodeGUID)
            {
                currentNode = nodeData;
                break;
            }
        }
    }

    private void Update()
    {
        //Play current node
        if(currentNode.nodeType == "Dialogue")
        {
            //TODO: Display dialogue on screen
            if (dialogueFinished)
            {
                findNextNodeInChain();
            }
        }
        else if(currentNode.nodeType == "Choice")
        {
            //TODO: Display choice buttons on screen 
            if (choiceChosen)
            {
                findNextNodeInChain();
            }
        }
        else if (currentNode.nodeType == "Pause")
        {
            if (!startTimeSet)
            {
                startTime = Time.realtimeSinceStartup;
            }
            else
            {
                if(Time.realtimeSinceStartup - startTime > currentNode.time)
                {
                    findNextNodeInChain();
                }
            }
        }
        else if (currentNode.nodeType == "Custom")
        {
            if (!checkRunning)
            {
                customCodeObject.SendMessage(currentNode.message);
                customCodeRunning = true;
                checkRunning = true;
            }  
            else if (!customCodeRunning)
            {
                findNextNodeInChain();
            }
        }
        else
        {
            findNextNodeInChain();
        }
    }

    public void SetChosenChoice(int enteredChoice)
    {
        chosenOption = Mathf.Clamp(enteredChoice, 0, 3);
        choiceChosen = true;
    }

    public void SetComparatorValue(float enteredValue)
    {
        comparatorValue = enteredValue;
    }
}
