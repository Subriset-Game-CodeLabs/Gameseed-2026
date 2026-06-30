using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public int CharacterSmashPower;
    public BattleInventory BattleInventory;
}

