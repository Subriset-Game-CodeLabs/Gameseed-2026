using UnityEngine;

[CreateAssetMenu(fileName = "BattleInventory", menuName = "Inventory/Battle Inventory")]
public class BattleInventory : ScriptableObject
{
    [Header("Selected Stick (1)")]
    public StickItem selectedStick;

    [Header("Selected Skills (max 3)")]
    public SkillItem[] selectedSkills = new SkillItem[3];

    public void Clear()
    {
        selectedStick = null;
        for (int i = 0; i < selectedSkills.Length; i++)
            selectedSkills[i] = null;
    }

    public bool IsSkillSelected(SkillItem skill)
    {
        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] == skill)
                return true;
        }
        return false;
    }

    public int GetSelectedSkillCount()
    {
        int count = 0;
        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] != null)
                count++;
        }
        return count;
    }

    public bool AreAllSkillsSelected()
    {
        return GetSelectedSkillCount() == 1;
    }

    public int AddSkill(SkillItem skill)
    {
        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] == null)
            {
                selectedSkills[i] = skill;
                return i;
            }
        }
        return -1;
    }

    public void RemoveSkillAt(int index)
    {
        if (index < 0 || index >= selectedSkills.Length) return;

        selectedSkills[index] = null;

        // Restack: shift non-null skills forward
        SkillItem[] temp = new SkillItem[3];
        int writeIdx = 0;
        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] != null)
            {
                temp[writeIdx] = selectedSkills[i];
                writeIdx++;
            }
        }
        for (int i = 0; i < 1; i++)
                selectedSkills[i] = temp[i];
    }

    public int GetSkillIndex(SkillItem skill)
    {
        for (int i = 0; i < selectedSkills.Length; i++)
        {
            if (selectedSkills[i] == skill)
                return i;
        }
        return -1;
    }
}
