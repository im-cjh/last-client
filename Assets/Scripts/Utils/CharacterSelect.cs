using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private Button[] characterButtons;
    private int selectedCharacterIndex = 0;
    private string prefabId;
    void Start()
    {
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => SelectedCharacter(index));
        }

        UpdateSelectionUI();
    }

    public void SelectedCharacter(int index)
    {
        selectedCharacterIndex = index;
        UpdateSelectionUI();

        switch (index)
        {
            case 0:
                prefabId = "Red";
                break;
            case 1:
                prefabId = "Shark";
                break;
            case 2:
                prefabId = "Malang";
                break;
            case 3:
                prefabId = "Frog";
                break;
        }

        PlayerInfoManager.instance.prefabId = prefabId;
    }

    private void UpdateSelectionUI()
    {
        frame.transform.position = characterButtons[selectedCharacterIndex].transform.position;
    }
}
