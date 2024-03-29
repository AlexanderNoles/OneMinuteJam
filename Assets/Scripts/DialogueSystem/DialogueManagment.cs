using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DialogueManagment : MonoBehaviour
{
    private NodeContainer _cachedContainer;
    private NodeData currentNode;

    //Dialogue
    public static bool dialogueFinished;
    [Header("Dialogue")]
    public GameObject dialogueBox;

    //Choice  
    private int chosenOption;
    private bool choiceChosen;
    [Header("Choice")]
    public GameObject threeChoicesObject;
    private AudioSource buttonClick;
    public TextMeshProUGUI[] threeButtonTexts;
    public GameObject[] threeActionPrompts;
    public GameObject twoChoicesObject;
    public TextMeshProUGUI[] twoButtonTexts;
    public GameObject[] twoActionPrompts;
    public GameObject timeBarEmpty;
    public RectTransform choiceTimeBar;
    public float timeGivenForChoice = 5f;
    private float timeLeft;
    public static float extraTime;

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
        buttonClick = GetComponent<AudioSource>();
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
            foreach (NodeLinkData nodeLinkData in _cachedContainer.edges)
            {
                if (int.Parse(nodeLinkData.PortNumber) == chosenOption + 1 && nodeLinkData.BaseNodeGUID == currentNode.GUID)
                {
                    FindTargetNode(nodeLinkData);
                    break;
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
                        if (int.Parse(nodeLinkData.PortNumber) == i+1 && nodeLinkData.BaseNodeGUID == currentNode.GUID)
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
            timeBarEmpty.SetActive(false);
            return;
        }

        //Reset Values
        choiceChosen = false;
        checkRunning = false;
        startTimeSet = false;
        NodeSetup();
    }

    private void NodeSetup()
    {
        if(currentNode.nodeType == "Dialogue")
        {
            dialogueBox.SetActive(true);
            dialogueFinished = false;
            dialogueBox.SendMessage("Play", currentNode.DialogueText);
        }
        else if (currentNode.nodeType == "Choice")
        {
            dialogueBox.SetActive(true);
            List<string> formattedOptions = currentNode.options.Where(x => x != "").ToList();
            int amountOfOptions = formattedOptions.Count();
            threeChoicesObject.SetActive(amountOfOptions == 3);
            twoChoicesObject.SetActive(amountOfOptions == 2);
            for (int i = 0; i < formattedOptions.Count; i++)
            {
                if(amountOfOptions == 3)
                {
                    threeButtonTexts[i].text = currentNode.options[i].Replace("[","").Replace("]","");
                    threeActionPrompts[i].SetActive(currentNode.options[i][0] == '[');
                }
                else if(amountOfOptions == 2)
                {
                    twoButtonTexts[i].text = currentNode.options[i].Replace("[", "").Replace("]", "");
                    twoActionPrompts[i].SetActive(currentNode.options[i][0] == '[');
                }
            }
            timeBarEmpty.SetActive(true);
            choiceTimeBar.sizeDelta = new Vector2(100, 125.16f);
            choiceTimeBar.anchoredPosition = new Vector2(0, 527.51f);
            timeLeft = timeGivenForChoice;
            chosenOption = 0;
            choiceChosen = false;
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
                float percentage = timeLeft / timeGivenForChoice;
                choiceTimeBar.sizeDelta = new Vector2(25 * (percentage), 0.5f);
                choiceTimeBar.anchoredPosition = new Vector2(-12.5f * (1 - (percentage)), 0f);
                if (choiceChosen || timeLeft < 0)
                {                 
                    threeChoicesObject.SetActive(false);
                    twoChoicesObject.SetActive(false);
                    timeBarEmpty.SetActive(false);
                    dialogueBox.SetActive(false);
                    if(timeLeft > 0)
                    {
                        extraTime += timeLeft;
                    }
                    findNextNodeInChain();
                }
                else
                {
                    SetComparatorValue(percentage);
                }
            }
            else if (currentNode.nodeType == "Pause")
            {
                if (!startTimeSet)
                {
                    startTime = Time.realtimeSinceStartup;
                    startTimeSet = true;
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
                    customCodeRunning = true;
                    checkRunning = true;
                    customCodeObject.SendMessage(currentNode.message);
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
        buttonClick.Play();
        chosenOption = Mathf.Clamp(enteredChoice, 0, 3);
        choiceChosen = true;
    }

    public void SetComparatorValue(float enteredValue)
    {
        comparatorValue = enteredValue;
    }
}
