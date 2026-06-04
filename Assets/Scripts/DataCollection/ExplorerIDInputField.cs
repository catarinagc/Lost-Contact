using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplorerIDInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        // Load previously saved value
        inputField.text = ExplorerIDManager.Instance.explorerID.ToString();

        // Listen for changes
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onValueChanged.AddListener(CheckInput);
        CheckInput(inputField.text);
    }

    private void CheckInput(string text)
    {
        bool validNumber = int.TryParse(text, out int value) && value > 0;
        startButton.interactable = validNumber;
    }

    private void OnValueChanged(string value)
    {
        if (int.TryParse(value, out int number))
        {
            ExplorerIDManager.Instance.explorerID = number;
        }
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(OnValueChanged);
        inputField.onValueChanged.RemoveListener(CheckInput);
    }
}
