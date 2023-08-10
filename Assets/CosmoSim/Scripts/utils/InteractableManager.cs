using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public enum highlightMovement
	{
        none,
        sine
	};

    public string radioGroupName;

    public Highlighters.Highlighter highlighter;
    public bool enabledOnStart = false;
    public highlightMovement blinkMode;
    public float blinkSpeed = 1f;
    public Vector2 minMaxIntesity;

    public Transform target;

    private float sineDelta=0f;
    private bool mouseIn = false;

    // Start is called before the first frame update
    void Start()
    {
        if(radioGroupName.Length>0)
		{
            if(!InteractableRadioGroupManager.instance.radioGroups.ContainsKey(radioGroupName))
                InteractableRadioGroupManager.instance.CreateRadioGroup(radioGroupName);
            InteractableRadioGroupManager.instance.AddElementToGroup(radioGroupName, this);            
        }

        if (enabledOnStart)
            EnableHighlighter();
        else
            DisableHighlighter();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseIn && blinkMode==highlightMovement.sine && highlighter!=null)
        {
            
            float calculatedWave = minMaxIntesity.x + ((minMaxIntesity.y - minMaxIntesity.x) * (1 - (Mathf.Cos(sineDelta) + 1f) / 2f));
            //Debug.Log(calculatedWave + " - " + sineDelta + " - " + (1 - (Mathf.Sin(sineDelta) + 1f) / 2f));
            sineDelta += Time.deltaTime * blinkSpeed;

            highlighter.Settings.BlurIntensity = calculatedWave;
            
        }
    }

    void OnMouseEnter()
    {
        sineDelta = 0;
        EnableHighlighter();
        mouseIn = true;
    }

    // ...the red fades out to cyan as the mouse is held over...
    void OnMouseOver()
    {
        
    }

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit()
    {
        DisableHighlighter();
        mouseIn = false;
    }

    void OnMouseDown()
    {
        GameManager.gameManager.ClickHud(target, radioGroupName);
    }

    private void EnableHighlighter()
    {
        if (highlighter!=null && !highlighter.enabled)
        {
            highlighter.enabled = true;
        }
    }

    // Method to disable the highlighter
    private void DisableHighlighter()
    {
        if(highlighter != null)
            highlighter.enabled = false;
    }
}
