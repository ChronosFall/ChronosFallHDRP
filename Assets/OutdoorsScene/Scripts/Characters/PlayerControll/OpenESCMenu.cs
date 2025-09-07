using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class OpenESCMenu : MonoBehaviour
{
    public Volume escMenuVolume;
    public bool isMenuActive = false;

    private DepthOfField depthOfField;

    void Start()
    {
        escMenuVolume.profile.TryGet(out depthOfField);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeMenuState();
        }
    }

    public void ChangeMenuState()
    {
        if (isMenuActive)
        {
            Debug.Log("Closing ESC Menu");
            isMenuActive = false;
            depthOfField.focusMode.value = DepthOfFieldMode.Off;
        }
        else
        {
            // Logic to open the menu
            Debug.Log("Opening ESC Menu");
            isMenuActive = true;
            depthOfField.focusMode.value = DepthOfFieldMode.Manual;
        }
    }
}