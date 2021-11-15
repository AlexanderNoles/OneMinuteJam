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
    [Header("Dialogue")]
    public static bool dialogueFinished;
    public GameObject dialogueBox;

    //Choice
    [Header("Choice")]
    private int chosenOption;
    private bool choiceChosen;
    public GameObject threeChoicesObject;
    public GameObject twoChoicesObject;
    public RectTransform choiceTimeBar;
    public float timeGivenForChoice = 5f;
    private float timeLeft;

    //Pause
    private float startTime;
    private bool startTimeSet;

    //Comparison
    private float comparatorValue;

    //Custom
    [Header("Custom Code")]
    public GameObject customCodeObject;
    public static bool customCodeRunning;
    private bool checkRunning;

    private bool running;

    private void Awake()
    {
        running = true;
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
        if(currentNode == null)
        {
            Debug.Log("No Starting Node");
        }    
    }

    private void findNextNodeInChain()
    {
        string cachedGUID = currentNode.GUID;
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

        if(currentNode.GUID == cachedGUID)
        {
            running = false;
            dialogueBox.SetActive(false);
            threeChoicesObject.SetActive(false);
            twoChoicesObject.SetActive(false);
            choiceTimeBar.gameObject.SetActive(false);
            return;
        }

        //Reset Values
        choiceChosen = false;
        checkRunning = false;
        startTimeSet = false;
        //Debug.Log(currentNode.nodeType);
        NodeSetup();
    }

    private void NodeSetup()
    {
        if(currentNode.nodeType == "Dialogue")
        {
            dialogueBox.SetActive(true);
            dialogueBox.SendMessage("Play", currentNode.DialogueText);
        }
        else if (currentNode.nodeType == "Choice")
        {
            dialogueBox.SetActive(true);
            int amountOfOptions = currentNode.options.Where(x => x != "").Count();
            threeChoicesObject.SetActive(amountOfOptions == 3);
            twoChoicesObject.SetActive(amountOfOptions == 2);
            choiceTimeBar.gameObject.SetActive(true);
            choiceTimeBar.sizeDelta = new Vector2(100, 125.16f);
            choiceTimeBar.anchoredPosition = new Vector2(0, 527.51f);
            timeLeft = timeGivenForChoice;
        }
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
        if (running)
        {
            //Play current node
            if (currentNode.nodeType == "Dialogue")
            {
                if (dialogueFinished)
                {
                    dialogueBox.SetActive(false);
                    findNextNodeInChain();
                }
            }
            else if (currentNode.nodeType == "Choice")
            {
                timeLeft -= Time.deltaTime;
                choiceTimeBar.sizeDelta = new Vector2(100 * (timeLeft / timeGivenForChoice), 125.16f);
                choiceTimeBar.anchoredPosition = new Vector2(-1000 * (1 - (timeLeft / timeGivenForChoice)), 527.51f);
                if (choiceChosen || timeLeft < 0)
                {
                    threeChoicesObject.SetActive(false);
                    twoChoicesObject.SetActive(false);
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
                    if (Time.realtimeSinceStartup - startTime > currentNode.time)
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
