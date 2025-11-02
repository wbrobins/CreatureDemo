using UnityEngine;

public class Dialogue
{
    private string speaker;
    private string text;

    public string Speaker => speaker;
    public string Text => text;

    public Dialogue(string pSpeaker, string pText)
    {
        speaker = pSpeaker;
        text = pText;
    }
}
