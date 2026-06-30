using UnityEngine;

public class SkillInstance
{
    public SkillItem Data;
    public int remainingCooldown;

    public bool IsReady() => remainingCooldown <= 0;
    public void Tick() => remainingCooldown = Mathf.Max(0, remainingCooldown - 1);
}
