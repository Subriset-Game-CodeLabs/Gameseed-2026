using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPopup;
    public GameObject creditsPopup;

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Scene Settings")]
    public string firstSceneName = "CoinFlipScene";

    void Start()
    {
        // Ambil data volume yang tersimpan, jika tidak ada pakai default 0.75f (75%)
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // Set nilai slider agar sesuai dengan data terakhir
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        // Terapkan ke Mixer
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    // --- MAIN MENU BUTTONS ---
    public void PlayGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }

    // --- POP-UP TOGGLES ---
    public void OpenSettings() => settingsPopup.SetActive(true);
    public void CloseSettings() => settingsPopup.SetActive(false);

    public void OpenCredits() => creditsPopup.SetActive(true);
    public void CloseCredits() => creditsPopup.SetActive(false);

    // --- SETTINGS LOGIC ---
    public void SetMusicVolume(float value)
    {
        // Mixer menggunakan Logaritma (db). Rumus: log10(value) * 20
        mainMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    public void SaveGameData()
    {
        PlayerPrefs.Save();
        Debug.Log("Game Settings & Progress Saved!");
        // Kamu bisa menambahkan logika save progress game-mu di sini nanti
    }
}