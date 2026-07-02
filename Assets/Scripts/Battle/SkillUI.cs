using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _skillObject;
    [SerializeField]
    private GameObject[] _listCooldownUI;
    private SkillInstance[] _listSkill;
    public event Action<SkillInstance> OnSkillPressed;


    public void SetupUI(SkillInstance[] skillItems, bool isPlayerUI = false)
    {
        _listSkill = skillItems;
        for (var i = 0; i < _skillObject.Length; i++)
        {
            TMP_Text skillText = _skillObject[i].GetComponentInChildren<TMP_Text>();
            skillText.text = skillItems[i].Data.itemName;
            Image skillImage = _skillObject[i].GetComponent<Image>();
            skillImage.sprite = skillItems[i].Data.icon;
            Button skillButton = _skillObject[i].GetComponent<Button>();

            if (isPlayerUI)
            {
                int index = i;
                skillButton.onClick.AddListener(() =>
                {

                    OnSkillPressed?.Invoke(skillItems[index]);
                });
            }
            else
            {
                skillButton.interactable = false;
            }

            UpdateUI(skillItems[i]);
        }
    }

    public void UpdateUI(SkillInstance skill)
    {
        int skillIndex = Array.IndexOf(_listSkill, skill);
        Button skillButton = _skillObject[skillIndex].GetComponent<Button>();

        if (skill.IsReady())
        {
            _listCooldownUI[skillIndex].SetActive(false);
            skillButton.interactable = true;
        }
        else
        {
            _listCooldownUI[skillIndex].SetActive(true);
            TMP_Text textCooldown = _listCooldownUI[skillIndex].GetComponentInChildren<TMP_Text>();
            textCooldown.text = skill.remainingCooldown.ToString();

            skillButton.interactable = false;
        }
    }
}
