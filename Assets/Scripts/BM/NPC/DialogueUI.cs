using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public Image npcPortrait;           // NPC portrait image.
    public TextMeshProUGUI dialogueText;  // Dialogue text.
    public TextMeshProUGUI characterNameText;  // NPC name text.
    public GameObject dialogueBox;      // The dialogue panel.

    void Start()
    {
        dialogueBox.SetActive(false);
        dialogueText.enableWordWrapping = true;
        dialogueText.overflowMode = TextOverflowModes.Overflow;
    }

    public void ShowDialogue(string characterName, string message, Sprite portrait)
    {
        characterNameText.text = characterName;
        dialogueText.text = message;
        npcPortrait.sprite = portrait;
        dialogueBox.SetActive(true);
    }

    public void HideDialogue()
    {
        characterNameText.text = "";
        dialogueText.text = "";
        npcPortrait.sprite = null;
        dialogueBox.SetActive(false);
    }
}
