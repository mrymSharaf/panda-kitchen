using UnityEngine;
using TMPro;
using System.Collections;

public class UIMessage : MonoBehaviour
{
public static UIMessage instance;

    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private Coroutine messageRoutine;

    void Awake()
    {
        instance = this;

        if (messageText != null)
            messageText.text = "";
    }

    public void ShowMessage(string message)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        if (messageText != null)
            messageText.text = message;

        yield return new WaitForSeconds(messageDuration);

        if (messageText != null)
            messageText.text = "";
    }
}
