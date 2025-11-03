using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public UnityEvent dialogueEmpty;

    [SerializeField] TextMeshProUGUI dialogueSpeaker;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Button option1Button;
    [SerializeField] Button option2Button;
    [SerializeField] GameObject panel;

    public bool choice = false;

    private List<Dialogue> dialogueQueue = new List<Dialogue>();

    void Start()
    {
        if (dialogueEmpty == null)
        {
            dialogueEmpty = new UnityEvent();
        }
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void AddToQueue(string speaker, string text)
    {
        Dialogue dialogue = new Dialogue(speaker, text);
        dialogueQueue.Add(dialogue);
    }

    public void PlayNextInQueue()
    {
        Show();

        bool isEmpty = !dialogueQueue.Any();

        if (!isEmpty)
        {
            SetSpeaker(dialogueQueue[0].Speaker);
            SetDialogue(dialogueQueue[0].Text);
        }
        else
        {
            dialogueEmpty.Invoke();
            Hide();
        }

        if (choice == true)
        {
            option1Button.gameObject.SetActive(false);
            option2Button.GetComponentInChildren<TextMeshProUGUI>().text = ">";
        }
    }

    public void SetSpeaker(string speaker)
    {
        dialogueSpeaker.text = speaker;
    }

    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public void SetChoiceButtons(string b1, string b2)
    {
        option1Button.gameObject.SetActive(true);
        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = b1;
        option2Button.GetComponentInChildren<TextMeshProUGUI>().text  = b2;
    }

    public void OnDialogueProgressed()
    {
        dialogueQueue.Remove(dialogueQueue[0]);
        choice = false;
        PlayNextInQueue();
    }

    public void ConfirmSelected()
    {
        dialogueQueue.Remove(dialogueQueue[0]);
        choice = true;
        PlayNextInQueue();
    }
}
