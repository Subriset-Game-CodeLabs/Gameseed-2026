using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField]
    private CharacterData _playerCharacter;
    [SerializeField]
    private CharacterData _enemyCharacter;

    public CharacterData PlayerCharacter => _playerCharacter;
    public CharacterData EnemyCharacter => _enemyCharacter;

    public void SetEnemyCharacter(CharacterData characterData)
    {
        _enemyCharacter = characterData;
    }
}