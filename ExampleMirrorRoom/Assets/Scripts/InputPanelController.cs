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
    
    // Start is called before the first frame update
    void Start()
    {
        _colorBtnReady = btnReady.GetComponent<Image>();
        btnCameraLeft.interactable = false;
    }
    
    public void OnClickLeft()
    {
        btnCameraLeft.interactable = false;
        btnCameraRight.interactable = true;
        UpdatePlayerCameraPosition(playerCamera.transform.position.x - _cameraOffset);
    }

    public void OnClickRight()
    {
        btnCameraLeft.interactable = true;
        btnCameraRight.interactable = false;
        UpdatePlayerCameraPosition(playerCamera.transform.position.x + _cameraOffset);
    }
    
    public void HandleReadyButton() 
    {
        isReady = !isReady; 
        string textChange = isReady ? "NOT READY" : "READY";
        _colorBtnReady.color = SetColor();
        txtReady.text = textChange;
    }
    
    private Color SetColor()
    {
        var hex = (isReady ?  _redColor : _greenColor).Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }

    private void UpdatePlayerCameraPosition(float xPosition)
    {
        playerCamera.transform.position = new Vector3(xPosition, playerCamera.transform.position.y, playerCamera.transform.position.z);
    }
    
    public void AppyColorToInterfazePlayers(Color color)
    {
        foreach (var player in playersInterfaze)
        {
            player.GetComponent<Renderer>().material.color = color;
        }
    }
}
