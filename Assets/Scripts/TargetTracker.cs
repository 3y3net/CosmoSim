using UnityEngine;
using UnityEngine.UI;

public class TargetTracker : MonoBehaviour {
    private Image trackerImage;
    public Vector3 target;
    public float rotSpeed = 15;
    public bool tracking = true;
    public Camera cam;

	void Start () {
        trackerImage = transform.GetComponent<Image>();
        cam = GameObject.FindObjectOfType<StarCamera>().transform.GetComponent<Camera>();
	}
    void Update() {
        if (tracking)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotSpeed);
            Vector3 heading = target - cam.transform.position;
            //test if the object taged is on FOV
            if (Vector3.Dot(cam.transform.forward, heading) > 0)
            {
                trackerImage.enabled = true;
                Vector3 screenPos = cam.WorldToScreenPoint(target);
                trackerImage.rectTransform.anchoredPosition = new Vector3(screenPos.x, screenPos.y, 0);
            }
            else
                trackerImage.enabled = false;
        }
        else trackerImage.enabled = false;
	}
}