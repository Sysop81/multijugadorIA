using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanelController : MonoBehaviour
{
    [Header("Camera Selector Buttons")] 
    [SerializeField] private Camera playerCamera;
    [SerializeField] public Button btnCameraLeft;
    [SerializeField] public Button btnCameraRight;
    [SerializeField] GameObject[] playersInterfaze;
    
    [Header("Input Player Name")]
    [SerializeField] public TMP_InputField inputPlayerName;
    
    
    [Header("Ready and Start Player State")]
    [SerializeField] public Button btnReady;
    [SerializeField] public TextMeshProUGUI txtReady;
    [SerializeField] public Button btnStart;
    
    // Private variables
    private readonly float _cameraOffset = 130f;
    private Image _colorBtnReady;
    private bool isReady;
    private readonly string _redColor = "#CC6666";
    private readonly string _greenColor = "#498839";
    
    /// <summary>
    /// Method Start
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _colorBtnReady = btnReady.GetComponent<Image>();
        btnCameraLeft.interactable = false;
    }
    
    /// <summary>
    /// Method OnClickLeft
    /// This method handle the left button of the selector player
    /// </summary>
    public void OnClickLeft()
    {
        btnCameraLeft.interactable = false;
        btnCameraRight.interactable = true;
        UpdatePlayerCameraPosition(playerCamera.transform.position.x - _cameraOffset);
    }
    
    /// <summary>
    /// Method OnClickRight
    /// This method handle the right button of the selector player
    /// </summary>
    public void OnClickRight()
    {
        btnCameraLeft.interactable = true;
        btnCameraRight.interactable = false;
        UpdatePlayerCameraPosition(playerCamera.transform.position.x + _cameraOffset);
    }
    
    /// <summary>
    /// Method HandleReadyButton
    /// This method handles the color and text of the ready button
    /// </summary>
    public void HandleReadyButton() 
    {
        isReady = !isReady; 
        string textChange = isReady ? "NOT READY" : "READY";
        _colorBtnReady.color = SetColor();
        txtReady.text = textChange;
    }
    
    /// <summary>
    /// Method SetColor
    /// This method apply the current color to the ready button
    /// </summary>
    /// <returns></returns>
    private Color SetColor()
    {
        var hex = (isReady ?  _redColor : _greenColor).Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }
    
    /// <summary>
    /// Method UpdatePlayerCameraPosition
    /// THIs method move the camera in x axes
    /// </summary>
    /// <param name="xPosition"></param>
    private void UpdatePlayerCameraPosition(float xPosition)
    {
        playerCamera.transform.position = new Vector3(xPosition, playerCamera.transform.position.y, playerCamera.transform.position.z);
    }
    
    /// <summary>
    /// Method AppyColorToInterfazePlayers
    /// This method apply the player color to the interfaze players
    /// </summary>
    /// <param name="color"></param>
    public void AppyColorToInterfazePlayers(Color color)
    {
        foreach (var player in playersInterfaze)
        {
            player.GetComponent<Renderer>().material.color = color;
        }
    }
}
