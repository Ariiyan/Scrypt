using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public Transform hangar;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI completionText;

    private bool completed = false;

    void Start()
    {
        SetInstructionText("Go to the plane hangar");
        SetCompletionTextActive(false);
    }

    void Update()
    {
        if (!completed && Vector3.Distance(player.position, hangar.position) < 5)
        {
            completed = true;
            SetInstructionTextActive(false);
            SetCompletionTextActive(true);
            SetCompletionText("You've reached the hangar. Game complete!");
        }
    }

    private void SetInstructionText(string text)
    {
        instructionText.text = text;
    }

    private void SetInstructionTextActive(bool active)
    {
        instructionText.gameObject.SetActive(active);
    }

    private void SetCompletionText(string text)
    {
        completionText.text = text;
    }

    private void SetCompletionTextActive(bool active)
    {
        completionText.gameObject.SetActive(active);
    }
}
