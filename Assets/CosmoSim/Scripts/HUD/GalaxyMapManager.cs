using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyMapManager : MonoBehaviour
{
    public enum GalaxyView
    {
        TopView,
        SideView
    }

    public GameObject galaxyMapObject;
    public Image galaxyImage;

    public float zoomLevel = 1.0f;
    public Vector2 panning = Vector2.zero;
    public bool showNet = true;

    public GalaxyView galaxyView = GalaxyView.TopView;
    
    
    public Text sectorText;
    public Sprite galaxyTop, galaxySide;

    public LineRendererHUD gridHUD;

    Vector2 touchStart;
    Rect startRect;
    float targetZoom;
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;


    // Start is called before the first frame update
    void Start()
    {
        Vector3[] corners = new Vector3[4];
        galaxyImage.rectTransform.GetWorldCorners(corners);
        startRect = new Rect(corners[0], corners[2] - corners[0]);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] corners = new Vector3[4];
        galaxyImage.rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);

        float xPositionDeltaPoint = Input.mousePosition.x - newRect.x;
        float yPositionDeltaPoint = Input.mousePosition.y - newRect.y;

        //Debug.Log("The x position delta is: " + xPositionDeltaPoint);
        //Debug.Log("The y position delta is: " + yPositionDeltaPoint);
        ManagePanAndZoom();
    }

    private void LateUpdate()
    {
        
    }

    void ManagePanAndZoom()
    {
        //Calculates current position
        Vector3[] corners = new Vector3[4];
        galaxyImage.rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);

        float xPositionDeltaPoint = Input.mousePosition.x - newRect.x;
        float yPositionDeltaPoint = Input.mousePosition.y - newRect.y;      

        //Calculate desplacement
        if (Input.GetMouseButtonDown(0))
        {
            touchStart.x = xPositionDeltaPoint;
            touchStart.y = yPositionDeltaPoint;
        }        
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - new Vector2(xPositionDeltaPoint, yPositionDeltaPoint);
            galaxyImage.rectTransform.position -= direction;
        }

        //Read mouse wheel and apply zoom
        float increment = -Input.GetAxis("Mouse ScrollWheel");

        float zoomFactor = Mathf.Clamp(galaxyImage.rectTransform.localScale.x + increment, zoomOutMin, zoomOutMax);

        //calculates displacement when zoom
        Vector2 correction = new Vector2(xPositionDeltaPoint, yPositionDeltaPoint);
        Vector2 newPos = correction * (zoomFactor / galaxyImage.rectTransform.localScale.x);
        correction = newPos - correction;

        
        gridHUD.SetNewGridFactor(((galaxyImage.rectTransform.localScale.x-1) % 2) + 1);

        //Apply zoom
        zoom(zoomFactor);

        //Apply translation and correction to zoom to mouse position        
        Debug.Log( increment);


        float factor = Mathf.Clamp(galaxyImage.rectTransform.localScale.x, zoomOutMin, zoomOutMax);
        Vector2 limits = startRect.size * (factor -1f);
        
        Vector2 correctedPosition = galaxyImage.rectTransform.position;

        correctedPosition -= startRect.position;
        //Debug.Log(factor + " - " + limits + " - " + galaxyImage.rectTransform.position + " - " + correctedPosition);

        
        if (correctedPosition.x < -limits.x)
            correctedPosition.x = -limits.x;

        if (correctedPosition.x > 0)
            correctedPosition.x = 0;

        if (correctedPosition.y < -limits.y)
            correctedPosition.y = -limits.y;

        if (correctedPosition.y > 0)
            correctedPosition.y = 0;
        

        galaxyImage.rectTransform.position = correctedPosition +  startRect.position - correction;
    }

    void zoom(float factor)
    {     
        galaxyImage.rectTransform.localScale = new Vector3(factor, factor, 0);
    }
}
