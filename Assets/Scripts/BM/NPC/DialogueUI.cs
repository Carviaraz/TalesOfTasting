using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public Image npcPortrait;   // รูปภาพของ NPC
    public TextMeshProUGUI dialogueText;   // ข้อความบทสนทนา
    public TextMeshProUGUI characterNameText;   // ชื่อตัวละคร
    public GameObject dialogueBox;   // ตัวกล่องข้อความ

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
