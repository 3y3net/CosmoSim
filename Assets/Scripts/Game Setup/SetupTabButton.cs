using UnityEngine;
using UnityEngine.UI;

public class SetupTabButton : MonoBehaviour {
    public GameObject cam;
    private GameSetupManager gSM;

    void Start()
    {
        //Calls the TaskOnClick method when you click the Button
        transform.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        gSM = GameObject.FindObjectOfType<GameSetupManager>();
    }

    void TaskOnClick()
    {
        gSM.TabButtonPressed(cam);
    }
}
