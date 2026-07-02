using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private GameObject healthIndicatorPrefab;
    [SerializeField] private Transform healthIndicatorParent;

    private readonly List<GameObject> _healthIndicators = new List<GameObject>();
    private HealthComponent _healthComponent;

    public void Initialize(HealthComponent healthComponent)
    {
        _healthComponent = healthComponent;

        ClearIndicators();
        SpawnIndicators(_healthComponent.MaxHP);

        _healthComponent.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI();

        StartCoroutine(DisableLayoutAfterFrame());
    }

    private System.Collections.IEnumerator DisableLayoutAfterFrame()
    {
        yield return null;

        var layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
            layoutGroup.enabled = false;
    }

    private void SpawnIndicators(int count)
    {
        // Transform parent = healthIndicatorParent != null ? healthIndicatorParent : transform;

        for (int i = 0; i < count; i++)
        {
            GameObject indicator = Instantiate(healthIndicatorPrefab, transform);
            _healthIndicators.Add(indicator);
        }

    }

    private void ClearIndicators()
    {
        foreach (var indicator in _healthIndicators)
        {
            if (indicator != null)
                Destroy(indicator);
        }
        _healthIndicators.Clear();
    }

    private void OnDestroy()
    {
        if (_healthComponent != null)
            _healthComponent.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < _healthIndicators.Count; i++)
        {
            // Deactivate indicators beyond current HP
            _healthIndicators[i].SetActive(i < _healthComponent.CurrentHP);
        }
    }

}
