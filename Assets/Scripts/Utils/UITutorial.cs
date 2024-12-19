using UnityEngine;
using UnityEngine.UI;

public class UITutorial : UIBase
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;
    private int curentPageIndex = 0;

    void Start()
    {
        UpdatePage();

        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
    }

    private void UpdatePage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == curentPageIndex);
        }

        prevButton.interactable = curentPageIndex > 0;
        nextButton.interactable = curentPageIndex < pages.Length - 1;
    }

    private void NextPage()
    {
        if (curentPageIndex < pages.Length - 1)
        {
            curentPageIndex++;
            UpdatePage();
        }
    }

    private void PrevPage()
    {
        if (curentPageIndex > 0)
        {
            curentPageIndex--;
            UpdatePage();
        }
    }
}
