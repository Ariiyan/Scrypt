using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image crosshairImage;

    private void Update()
    {
        // Get the center position of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Set the position of the crosshair image to the center of the screen
        crosshairImage.rectTransform.position = screenCenter;
    }
}
