using UnityEngine;

[CreateAssetMenu(fileName = "CharacterList", menuName = "CharacterList")]
public class CharacterList : ScriptableObject
{
    public PlayerConfig[] Characters;
}
