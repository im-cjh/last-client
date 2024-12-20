using UnityEngine;
using UnityEngine.UI;

public class UITutorial : UIBase
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button closeButton;
    private int curentPageIndex = 0;

    private void UpdatePage()
    {
        pages[curentPageIndex].SetActive(true);

        //prevButton.interactable = (curentPageIndex != 0);
        //nextButton.interactable = (curentPageIndex != pages.Length - 1);        

        prevButton.gameObject.SetActive(curentPageIndex != 0);
        nextButton.gameObject.SetActive(curentPageIndex != pages.Length - 1);

    }

    public void NextPage()
    {
        pages[curentPageIndex].SetActive(false);
        curentPageIndex = Mathf.Clamp(curentPageIndex + 1, 0, pages.Length - 1);
        UpdatePage();
    }

    public void PrevPage()
    {
        pages[curentPageIndex].SetActive(false);
        curentPageIndex = Mathf.Clamp(curentPageIndex -1, 0, pages.Length - 1);
        UpdatePage();
    }
}
