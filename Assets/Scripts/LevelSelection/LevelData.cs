using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data/Levels")]

public class LevelData : ScriptableObject
{
  [Header("Level Stats")]
  public string LevelID;
  [Tooltip("For Starting Level")] public bool IsUnlockedByDefault;
  public SceneField Scene;

  [Header("Level Display Information")]
  public string LevelName;

  [Header("Level Description")]

  public Sprite LevelOverview;

  [TextArea(3, 6)]
  public string Description;
  public Sprite EnemySprite;
  public List<RewardData> Rewards;

  public CharacterData EnemyData;
  public LevelData NextLevelData;
  public GameObject LevelPrefab;

  public GameObject LevelButtonObj { get; set; }
}