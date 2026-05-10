using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuNavButton : MonoBehaviour
{
    [SerializeField]
    private MainMenuUIController controller;

    [SerializeField]
    private MenuPage targetPage;


    private void OnEnable()
    {
        if (TryGetComponent(out Button button))
            UnityEventUtility.RegisterListener(button, OnClick);
    }

    private void OnDisable()
    {
        if (TryGetComponent(out Button button))
            UnityEventUtility.UnregisterListener(button, OnClick);
    }


    public void OnClick()
    {
        if (controller != null && targetPage != null)
            controller.OpenPage(targetPage);
    }
}
