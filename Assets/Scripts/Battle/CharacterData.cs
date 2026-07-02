using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public int CharacterSmashPower;
    public int MaxHP = 5;
    public BattleInventory BattleInventory;
}

