using UnityEngine;
public class StellarText : MonoBehaviour {
    private Camera cam;
    private TextMesh textMesh;
	private void Start () {
        cam = GameObject.FindObjectOfType<Camera>();
        textMesh = GetComponentInChildren<TextMesh>();
        textMesh.text = GetComponentInParent<Star>().starName;
        textMesh.color = new Color(1f, 1f, 1f, 0.75f);
	}
	private void Update () {
        transform.rotation = cam.transform.rotation;
	}
}