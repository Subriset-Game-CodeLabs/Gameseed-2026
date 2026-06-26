using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Area", menuName = "Level Data/Areas")]
public class AreaData : ScriptableObject
{
    public string AreaName;
    public List<LevelData> Levels = new List<LevelData>();
}