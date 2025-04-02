using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] messages;      // Array of dialogue messages.
    public string characterName;   // NPC name.
    public Sprite npcPortrait;     // NPC portrait.

    private bool isPlayerNearby = false;
    private bool isDialogueOpen = false;
    private int currentMessageIndex = 0;  // Which message is being shown.
    public DialogueUI dialogueUI;         // Reference to the dialogue UI component.
    private Transform player;

    void Start()
    {
        // Initially hide dialogue UI.
        HideMessage();
    }

    void Update()
    {
        // When the player is nearby and F is pressed, advance dialogue.
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (!isDialogueOpen)
            {
                // Start dialogue at the beginning.
                currentMessageIndex = 0;
                ShowMessage();
            }
            else
            {
                // Advance to the next message.
                ShowNextMessage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.transform;
            // Optionally, show a "Press F to talk" prompt here.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            HideMessage();  // Close dialogue when player leaves.
        }
    }

    public void ShowMessage()
    {
        if (dialogueUI != null && messages.Length > 0)
        {
            dialogueUI.ShowDialogue(characterName, messages[currentMessageIndex], npcPortrait);
            isDialogueOpen = true;
        }
    }

    public void ShowNextMessage()
    {
        currentMessageIndex++;

        if (currentMessageIndex >= messages.Length)
        {
            // End dialogue if we've shown all messages.
            HideMessage();
        }
        else
        {
            dialogueUI.ShowDialogue(characterName, messages[currentMessageIndex], npcPortrait);
        }
    }

    public void HideMessage()
    {
        if (dialogueUI != null)
        {
            dialogueUI.HideDialogue();
            isDialogueOpen = false;
            currentMessageIndex = 0;
        }
    }
}
