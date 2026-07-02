using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField]
    private CharacterData _playerCharacter;
    [SerializeField]
    private CharacterData _enemyCharacter;
    [SerializeField]
    private LevelData _selectedLevelData;

    [SerializeField]
    private List<LevelData> _unlockedLevel;

    public CharacterData PlayerCharacter => _playerCharacter;
    public CharacterData EnemyCharacter => _enemyCharacter;
    public LevelData LevelData => _selectedLevelData;
    public List<LevelData> UnlockedLevel => _unlockedLevel;

    public void SetLevelData(LevelData levelData)
    {
        _selectedLevelData = levelData;
        SetEnemyCharacter(levelData.EnemyData);
    }

    public void SetEnemyCharacter(CharacterData characterData)
    {
        _enemyCharacter = characterData;
    }

    public void AddUnlockedLevel(LevelData level)
    {
        _unlockedLevel.Add(level);
    }

    public void PlayerWin()
    {
        // applied Rewards
        if (_selectedLevelData.NextLevelData != null)
        {
            _unlockedLevel.Add(_selectedLevelData.NextLevelData);
        }
    }


}