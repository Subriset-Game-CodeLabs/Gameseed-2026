using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PrologManager : MonoBehaviour
{
    [Header("UI Narasi (Fase 1)")]
    public GameObject narrativePanel;
    public TextMeshProUGUI narrativeText; 
    [TextArea(3, 5)]
    [Tooltip("Isi dengan beberapa paragraf cerita. Pemain akan lanjut ke paragraf berikutnya saat klik.")]
    public string[] narrativeStrings;

    [Header("UI Karakter & Bubble (Fase 2)")]
    public Animator characterAnimator;
    public GameObject bubbleChatObject;
    public Animator bubbleAnimator;
    public TextMeshProUGUI bubbleText; 
    [TextArea(3, 5)]
    [Tooltip("Isi dengan beberapa kalimat bubble. Pemain akan lanjut ke kalimat berikutnya saat klik.")]
    public string[] bubbleStrings;

    [Header("Navigasi")]
    public Button lanjutButton;
    public string nextSceneName = "CoinFlipScene";
    public float typingSpeed = 0.05f;

    // Variabel Internal
    private int currentPhase = 1; 
    private int currentLineIndex = 0; 
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Sembunyikan elemen Fase 2 dan tombol Lanjut di awal
        bubbleChatObject.SetActive(false);
        lanjutButton.gameObject.SetActive(false);

        // Mulai Fase 1
        StartPhase1();
    }

    void Update()
    {
        // Deteksi klik mouse sebelah kiri menggunakan UnityEngine.Input
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // JIKA TEKS SEDANG NGETIK -> SKIP (Tampilkan semua huruf)
                StopCoroutine(typingCoroutine);
                
                if (currentPhase == 1)
                {
                    narrativeText.text = narrativeStrings[currentLineIndex];
                }
                else if (currentPhase == 2)
                {
                    bubbleText.text = bubbleStrings[currentLineIndex];
                }
                
                isTyping = false;
            }
            else
            {
                // JIKA TEKS SUDAH TAMPIL SEMUA -> LANJUT KE KALIMAT BERIKUTNYA
                currentLineIndex++; 

                if (currentPhase == 1)
                {
                    // Cek apakah masih ada kalimat narasi tersisa
                    if (currentLineIndex < narrativeStrings.Length)
                    {
                        typingCoroutine = StartCoroutine(TypeWriterEffect(narrativeText, narrativeStrings[currentLineIndex]));
                    }
                    else
                    {
                        // Jika narasi habis, otomatis masuk ke Fase 2
                        StartPhase2();
                    }
                }
                else if (currentPhase == 2)
                {
                    // Cek apakah masih ada kalimat bubble tersisa
                    if (currentLineIndex < bubbleStrings.Length)
                    {
                        typingCoroutine = StartCoroutine(TypeWriterEffect(bubbleText, bubbleStrings[currentLineIndex]));
                    }
                    else
                    {
                        // Jika dialog bubble habis, munculkan tombol Lanjut
                        lanjutButton.gameObject.SetActive(true);
                        
                        // Matikan script ini agar klik layar tidak melakukan apa-apa lagi
                        this.enabled = false; 
                    }
                }
            }
        }
    }

    private void StartPhase1()
    {
        currentPhase = 1;
        currentLineIndex = 0; 
        narrativePanel.SetActive(true);
        
        // Pastikan narrativeText aktif di awal jika sebelumnya sempat mati
        if (narrativeText != null)
        {
            narrativeText.gameObject.SetActive(true);
        }

        if (narrativeStrings.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeWriterEffect(narrativeText, narrativeStrings[currentLineIndex]));
        }
        else
        {
            StartPhase2();
        }
    }

    private void StartPhase2()
    {
        currentPhase = 2;
        currentLineIndex = 0; 

        // [PERBAIKAN] Hanya menonaktifkan Game Object dari Text-nya saja, Panel tetap aktif
        if (narrativeText != null)
        {
            narrativeText.gameObject.SetActive(false);
        }

        // Munculkan Karakter
        characterAnimator.gameObject.SetActive(true);
        characterAnimator.SetTrigger("ShowCharacter");

        // Munculkan Bubble
        bubbleChatObject.SetActive(true);
        bubbleAnimator.SetTrigger("ShowBubble");

        if (bubbleStrings.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeWriterEffect(bubbleText, bubbleStrings[currentLineIndex]));
        }
        else
        {
            lanjutButton.gameObject.SetActive(true);
            this.enabled = false;
        }
    }

    private IEnumerator TypeWriterEffect(TextMeshProUGUI targetText, string fullText)
    {
        isTyping = true;
        targetText.text = "";

        foreach (char c in fullText.ToCharArray())
        {
            targetText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}