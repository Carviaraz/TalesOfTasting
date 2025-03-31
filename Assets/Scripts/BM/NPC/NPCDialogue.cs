using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 5)]
    public string[] messages;  // เก็บหลายบทสนทนา
    public string characterName;  // ชื่อตัวละคร
    public Sprite npcPortrait;  // รูปภาพของ NPC

    private bool isPlayerNearby = false;
    private bool isDialogueOpen = false;
    private int currentMessageIndex = 0;  // ตำแหน่งบทสนทนา
    public float closeDistance = 3.0f;  // ระยะห่างที่ข้อความจะปิด
    public DialogueUI dialogueUI;
    private Transform player;

    void Start()
    {
        HideMessage();
    }

    void Update()
    {
        // ตรวจสอบระยะห่างระหว่าง NPC กับผู้เล่น
        if (isDialogueOpen && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > closeDistance)
            {
                HideMessage();
            }
        }

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (isDialogueOpen)
            {
                ShowNextMessage();
            }
            else
            {
                currentMessageIndex = 0;  // เริ่มต้นบทสนทนาใหม่
                ShowMessage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
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
            HideMessage();  // ปิดข้อความถ้าบทสนทนาหมดแล้ว
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
