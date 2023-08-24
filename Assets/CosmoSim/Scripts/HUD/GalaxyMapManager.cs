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
    public Image crosshair_v, crosshair_h;


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

    public Text text_x, text_y;


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

        
        ManagePanAndZoom(xPositionDeltaPoint, yPositionDeltaPoint, newRect);

        PositionCrtosshaiTopView(xPositionDeltaPoint, yPositionDeltaPoint, newRect);        
    }

    private void LateUpdate()
    {
        
    }

    void ManagePanAndZoom(float xPositionDeltaPoint, float yPositionDeltaPoint, Rect newRect)
    {
        
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
        float increment = Input.GetAxis("Mouse ScrollWheel");

        zoomLevel = Mathf.Clamp(galaxyImage.rectTransform.localScale.x + increment, zoomOutMin, zoomOutMax);

        //calculates displacement when zoom
        Vector2 correction = new Vector2(xPositionDeltaPoint, yPositionDeltaPoint);
        Vector2 newPos = correction * (zoomLevel / galaxyImage.rectTransform.localScale.x);
        correction = newPos - correction;

        
        gridHUD.SetNewGridFactor(((galaxyImage.rectTransform.localScale.x-1) % 2) + 1);

        //Apply zoom
        zoom(zoomLevel);

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
        panning = new Vector2(galaxyImage.rectTransform.position.x -startRect.x, galaxyImage.rectTransform.position.y-startRect.y);// - newRect.position;
    }

    void zoom(float factor)
    {     
        galaxyImage.rectTransform.localScale = new Vector3(factor, factor, 0);
    }

    public void PositionCrtosshaiTopView(float x, float y, Rect origin)
	{
                   
        float range_x = (x * 10000f / 1480f)-5000f;
        float range_y = (y * 10000f / 980f) - 5000f;

        text_x.text = range_x.ToString();
        text_y.text = range_y.ToString();

        /*
        x = x < 10 ? 10 : x > 1490 ? 1490 : x;
        y = y <= 10 ? 10 : y > 990 ? 990 : y;
        */

        float posx = x + startRect.x - 10 + panning.x;
        float posy = y + startRect.y - 10 + panning.y;
        
        posx = posx < startRect.x ? startRect.x : posx > 1500+startRect.x-20 ? 1500+startRect.x-20 : posx;
        posy = posy < startRect.y ? startRect.y : posy > 1000 + startRect.y - 20 ? 1000 + startRect.y - 20 : posy;

        //Debug.Log(x + " - " + y + " || " + origin + " - " + posx + ", " + posy);

        crosshair_h.rectTransform.position = new Vector3(origin.x - panning.x, posy, 0);
        crosshair_v.rectTransform.position = new Vector3(posx, origin.y - panning.y, 0);
    }
}
