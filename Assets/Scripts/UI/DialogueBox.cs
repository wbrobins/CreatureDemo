using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueBox : MonoBehaviour
{
    public UnityEvent dialogueEmpty;

    [SerializeField] TextMeshProUGUI dialogueSpeaker;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject panel;

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
    }

    public void SetSpeaker(string speaker)
    {
        dialogueSpeaker.text = speaker;
    }

    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public void OnDialogueProgressed()
    {
        dialogueQueue.Remove(dialogueQueue[0]);
        PlayNextInQueue();
    }
}
