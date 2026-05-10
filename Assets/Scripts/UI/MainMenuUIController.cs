using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Starting Page")]
    [SerializeField]
    private MenuPage landingPage;


    [Header("Buttons")]
    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Button pcButton;

    [SerializeField]
    private Button webButton;


    private MenuPage currentPage;
    private bool isTransitioning;


    private void OnEnable()
    {
        UnityEventUtility.RegisterListener(quitButton, OnQuitButtonClicked);
        UnityEventUtility.RegisterListener(pcButton, OnPCButtonClicked);
        UnityEventUtility.RegisterListener(webButton, OnWebButtonClicked);
    }

    private void OnDisable()
    {
        UnityEventUtility.UnregisterListener(quitButton, OnQuitButtonClicked);
        UnityEventUtility.UnregisterListener(pcButton, OnPCButtonClicked);
        UnityEventUtility.UnregisterListener(webButton, OnWebButtonClicked);
    }

    private async void Start()
    {
        currentPage = landingPage;

        if (currentPage != null)
            await currentPage.Enter();
    }

    public async void OpenPage(MenuPage targetPage)
    {
        if (isTransitioning)
            return;

        if (targetPage == null)
            return;

        if (targetPage == currentPage)
            return;

        isTransitioning = true;

        if (currentPage != null)
            await currentPage.Exit();

        currentPage = targetPage;

        if (currentPage != null)
            await currentPage.Enter();

        isTransitioning = false;
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnPCButtonClicked()
    {
        SceneManager.LoadScene("MapView");
    }

    private void OnWebButtonClicked()
    {
        Application.OpenURL("www.youtube.com");
    }
}
