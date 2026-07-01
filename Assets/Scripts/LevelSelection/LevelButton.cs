using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNameText;

    public LevelData LevelData { get; set; }

    private Button _button;
    private Image _image;
    private LevelDetailPanel _detailPanel;
    public Color ReturnColor { get; set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        ReturnColor = Color.grey;
    }

    public void Setup(LevelData level, bool isUnlocked, LevelDetailPanel detailPanel)
    {
        LevelData = level;
        _detailPanel = detailPanel;
        _levelNameText.SetText(level.LevelID);

        _button.interactable = isUnlocked;

        if (isUnlocked)
        {
            _button.onClick.AddListener(OpenDetailPanel);
            ReturnColor = Color.white;
            _image.color = ReturnColor;
        }
        else
        {
            ReturnColor = Color.grey;
            _image.color = ReturnColor;
        }
    }

    public void Unlock()
    {
        _button.interactable = true;
        _button.onClick.AddListener(OpenDetailPanel);
        ReturnColor = Color.white;
        _image.color = ReturnColor;
    }

    private void OpenDetailPanel()
    {
        _detailPanel.Show(LevelData);
    }

    // public void LoadLevel()
    // {
    //     SceneManager.LoadScene(LevelData.Scene);
    // }
}