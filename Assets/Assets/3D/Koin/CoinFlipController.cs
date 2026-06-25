using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinFlipController : MonoBehaviour
{
    [Header("Referensi Object")]
    public GameObject coin3D;
    public Button confirmButton;
    public Animator coinAnimator;
    public Animator uiResultAnimator;
    [Tooltip("Masukkan Game Object UI/Text yang ingin di-hide saat koin pertama kali diklik")]
    public GameObject initialUIToHide; 

    [Header("Pengaturan Durasi")]
    public float flipAnimationDuration = 0.5f; // Durasi animasi membalik koin saat diklik
    public float tossAnimationDuration = 3f;   // Durasi animasi lemparan penentuan hasil
    public float resultAnimationDuration = 2f; // Durasi animasi UI Turn

    [Header("Pengaturan Scene")]
    public string gameplaySceneName = "GameplayScene";

    // Variabel Internal
    private bool hasClickedOnce = false;
    private bool isPlayerChoosingGambar = true; // Anggap awal mula koin menghadap "Gambar"
    private bool isFlipping = false; // Mencegah klik saat koin sedang berputar/animasi
    private bool hasConfirmed = false;

    void Update()
    {
        // Deteksi klik mouse sebelah kiri menggunakan UnityEngine.Input
        if (UnityEngine.Input.GetMouseButtonDown(0) && !isFlipping && !hasConfirmed)
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;

            // Jika Raycast mengenai 3D Koin
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == coin3D)
                {
                    StartCoroutine(FlipCoinAnimation());

                    // Jalankan ini HANYA pada klik pertama
                    if (!hasClickedOnce)
                    {
                        hasClickedOnce = true;
                        
                        // Munculkan tombol Confirm
                        if (confirmButton != null)
                            confirmButton.gameObject.SetActive(true);

                        // Hide UI/Text instruksi awal
                        if (initialUIToHide != null)
                            initialUIToHide.SetActive(false);
                    }
                }
            }
        }
    }

    // Coroutine untuk memutar koin menggunakan Animator saat player memilih sisi
    private IEnumerator FlipCoinAnimation()
    {
        isFlipping = true;

        // Cek sisi koin saat ini, lalu mainkan animasi ke sisi sebaliknya
        if (isPlayerChoosingGambar)
        {
            // Jika saat ini sedang di sisi Gambar, putar ke Angka
            coinAnimator.SetTrigger("FlipToAngka");
            isPlayerChoosingGambar = false; 
        }
        else
        {
            // Jika saat ini sedang di sisi Angka, putar ke Gambar
            coinAnimator.SetTrigger("FlipToGambar");
            isPlayerChoosingGambar = true;
        }

        // Tunggu sesuai durasi animasi memutar sebelum mengizinkan klik lagi
        yield return new WaitForSeconds(flipAnimationDuration);
        
        isFlipping = false;
    }

    // Dipanggil oleh Tombol Confirm UI
    public void OnConfirmClicked()
    {
        hasConfirmed = true;
        
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(false); // Sembunyikan tombol lagi

        StartCoroutine(ProcessCoinFlipSequence());
    }

    private IEnumerator ProcessCoinFlipSequence()
    {
        // 1. Sistem Random Menentukan Hasil Lemparan Koin (0 = Gambar, 1 = Angka)
        int randomCoinResult = Random.Range(0, 2); 
        bool coinLandedGambar = (randomCoinResult == 0);

        // 2. Play Animasi Lempar Koin sesuai SISI AWAL dan HASIL AKHIR
        if (isPlayerChoosingGambar && coinLandedGambar)
        {
            // Sisi awal Gambar -> Hasil akhir Gambar
            coinAnimator.SetTrigger("TossGambarToGambar");
        }
        else if (!isPlayerChoosingGambar && coinLandedGambar)
        {
            // Sisi awal Angka -> Hasil akhir Gambar
            coinAnimator.SetTrigger("TossAngkaToGambar");
        }
        else if (isPlayerChoosingGambar && !coinLandedGambar)
        {
            // Sisi awal Gambar -> Hasil akhir Angka
            coinAnimator.SetTrigger("TossGambarToAngka");
        }
        else if (!isPlayerChoosingGambar && !coinLandedGambar)
        {
            // Sisi awal Angka -> Hasil akhir Angka
            coinAnimator.SetTrigger("TossAngkaToAngka");
        }

        yield return new WaitForSeconds(tossAnimationDuration);

        // Penentuan siapa yang main duluan
        bool playerGoesFirst = (isPlayerChoosingGambar == coinLandedGambar);

        // Simpan data, dibaca di scene gameplay menggunakan PlayerPrefs
        // 1 = True (Player First), 0 = False (Enemy First)
        PlayerPrefs.SetInt("PlayerGoesFirst", playerGoesFirst ? 1 : 0);
        PlayerPrefs.Save();

        // Play Animasi UI Result
        if (playerGoesFirst)
            uiResultAnimator.SetTrigger("PlayerFirst");
        else
            uiResultAnimator.SetTrigger("EnemyFirst");

        yield return new WaitForSeconds(resultAnimationDuration);

        // Pindah ke Scene Gameplay
        SceneManager.LoadScene(gameplaySceneName);
    }
}