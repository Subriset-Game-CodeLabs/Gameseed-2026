using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _mainMenuButton;
    [SerializeField]
    private string _restartScene;
    [SerializeField]
    private string _mainMenuScene;


    void Start()
    {
        _restartButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene(_restartScene); 
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene(_mainMenuScene); 
        });
    }
}
