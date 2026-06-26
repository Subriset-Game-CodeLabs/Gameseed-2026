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

  public GameObject LevelButtonObj { get; set; }
}