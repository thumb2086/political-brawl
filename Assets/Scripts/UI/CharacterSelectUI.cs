using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject characterButtonPrefab;
    public Transform buttonContainer;
    public Button confirmButton;
    public Text selectedNameText;
    public Text selectedClassText;

    private int selectedCharacterIndex = -1;
    private int playerIndex;

    public struct CharacterInfo
    {
        public string name;
        public string className;
        public string description;

        public CharacterInfo(string name, string className, string description)
        {
            this.name = name;
            this.className = className;
            this.description = description;
        }
    }

    private List<CharacterInfo> characters = new List<CharacterInfo>
    {
        new CharacterInfo("川普", "坦克", "高血量、近戰強勢，築牆封路"),
        new CharacterInfo("習近平", "控場", "範圍攻擊、一帶一路路徑控場"),
        new CharacterInfo("賴清德", "射手", "遠程精準狙擊、團結台灣彈幕"),
        new CharacterInfo("蔡英文", "支援", "治療與傷害兼具、部署國家隊哨塔")
    };

    void Start()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            int index = i;
            GameObject btnObj = Instantiate(characterButtonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            Text btnText = btnObj.GetComponentInChildren<Text>();

            if (btnText != null)
                btnText.text = characters[i].name;

            btn.onClick.AddListener(() => SelectCharacter(index));
        }

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmSelection);

        UpdateDisplay();
    }

    void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characters.Count)
        {
            var info = characters[selectedCharacterIndex];
            if (selectedNameText != null)
                selectedNameText.text = info.name;
            if (selectedClassText != null)
                selectedClassText.text = $"{info.className} - {info.description}";
        }

        if (confirmButton != null)
            confirmButton.interactable = selectedCharacterIndex >= 0;
    }

    void ConfirmSelection()
    {
        if (selectedCharacterIndex < 0) return;

        GameManager.Instance.StartGame(new int[] { selectedCharacterIndex });
        gameObject.SetActive(false);
    }
}
