using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField]
    private Button backButton;


    private void OnEnable()
    {
        UnityEventUtility.RegisterListener(backButton, OnBackButtonClicked);
    }

    private void OnDisable()
    {
        UnityEventUtility.UnregisterListener(backButton, OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
