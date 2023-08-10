using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameMode
	{
        Cinematic,
        Galaxy,
        Star,
        Planet,
        HUD,
        Menu
	}

    public static GameManager gameManager;

    public GameMode gameMode = GameMode.Galaxy;

    public GalaxyCamera galaxyCamera;
    public StarCamera starCamera;
    public CockpitCamera cockpitCamera;

    public GameObject cockpitModel;

    GameMode previousMode;
    public Transform left, right, up, down, front;
    public string radioGroupName = "";

    private void Awake()
	{
        gameManager = this;	
	}

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
    }

    void LateUpdate()
    {
        ManageLateUpdate();
    }

    void ManageInput()
	{
        if (!cockpitModel.activeSelf)
            cockpitModel.SetActive(true);

        switch (gameMode)
		{
            case GameMode.Galaxy:
                if (Input.GetKey(KeyCode.F) && cockpitModel.activeSelf)
                {
                    cockpitModel.SetActive(false);
                }
                else if (Input.GetKey(KeyCode.L))
                {
                    cockpitCamera.SetTarget(left);
                }
                else if (Input.GetKey(KeyCode.R))
                {
                    cockpitCamera.SetTarget(right);
                }
                else if (Input.GetKey(KeyCode.T))
                {
                    cockpitCamera.SetTarget(up);
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    cockpitCamera.SetTarget(down);
                }
                else
                {
                    cockpitCamera.SetTarget(front, true);
                    starCamera.ManageInput();
                }

                break;

		}
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameMode == GameMode.HUD)
            {
                gameMode = previousMode;
                cockpitCamera.GoBack();
                if (InteractableRadioGroupManager.instance.radioGroups.ContainsKey(radioGroupName))
                {
                    InteractableRadioGroupManager.instance.EnableAllElementsFromGroup(radioGroupName);
                }
            }            
            else
                Application.Quit();
        }
        
    }


    void ManageLateUpdate()
	{
        switch (gameMode)
        {
            case GameMode.Galaxy:
                starCamera.ManageLateUpdate();
                break;

        }
    }

    public void ClickHud(Transform target, string radioGName)
	{
        previousMode = gameMode;
        gameMode = GameMode.HUD;
        cockpitCamera.SetTarget(target);
        if (InteractableRadioGroupManager.instance.radioGroups.ContainsKey(radioGName))
        {
            InteractableRadioGroupManager.instance.DisableAllElementsFromGroup(radioGName);
            radioGroupName = radioGName;
        }
    }
}
