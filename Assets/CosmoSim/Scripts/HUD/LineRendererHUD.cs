using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererHUD : Graphic
{

    public Vector2 gridSize;
    public Vector2 deltaOrigin;
    public float thickness=10f;
    public float gridFactor = 1f;

    float width;
    float height;
    float celltWidth;
    float cellHeight;

    public void SetNewGridFactor(float factor)
    {
        if (gridFactor == factor)
            return;
        gridFactor=factor;
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {

        vh.Clear();

        Vector2 finalGridSize =gridSize * gridFactor;

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        celltWidth = width / finalGridSize.x;
        cellHeight = height / finalGridSize.y;

        int count = 0;

        /* old method
        for(int y=0; y<gridSize.y; y++)
        {
            for(int x=0; x<gridSize.x; x++)
            {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
        */

        float xOrigin = deltaOrigin.x % finalGridSize.x;
        if (xOrigin < 0)
            xOrigin += finalGridSize.x;
        for (float x = xOrigin; x < width; x+= finalGridSize.x)
        {
            DrawYAxis((float)x, count, vh);
            count++;
        }

        float yOrigin = deltaOrigin.y % finalGridSize.y;
        if (yOrigin < 0)
            yOrigin += finalGridSize.y;

        for (float y = yOrigin; y < height; y += finalGridSize.y)
        {
            DrawXAxis((float)y, count, vh);
            count++;
        }

        /*
        for (int x = 0; x <= gridSize.x; x++)
        {
            DrawYAxis((float)x * celltWidth, count, vh);
            count++;
        }

        for (int y = 0; y <= gridSize.y; y++)
        {
            DrawXAxis((float)y * cellHeight, count, vh);
            count++;
        }
        */
    }


    private void DrawXAxis(float origin, int index, VertexHelper vh)
    {

        float xPos = 0;
        float yPos = origin;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + width, yPos );
        vh.AddVert(vertex);

        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr / 2f;

        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + width, yPos + distance);
        vh.AddVert(vertex);

        int offset = index * 4;

        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 1, offset + 3, offset + 2);

    }

    private void DrawYAxis(float origin, int index, VertexHelper vh)
    {

        float xPos = origin;
        float yPos = 0;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos, yPos + height);
        vh.AddVert(vertex);

        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr / 2f;

        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos + distance, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos + height);
        vh.AddVert(vertex);

        int offset = index * 4;

        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 1, offset + 3, offset + 2);

    }

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {

        float xPos = celltWidth * x;
        float yPos = cellHeight * y;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + celltWidth, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + celltWidth, yPos);
        vh.AddVert(vertex);

        //vh.AddTriangle(0, 1, 2);
        //vh.AddTriangle(2, 3, 0);

        float widthSqr = thickness * thickness;
        float distanceSqr = widthSqr / 2f;

        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos +(cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (celltWidth - distance), yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (celltWidth - distance), yPos + distance);
        vh.AddVert(vertex);

        int offset = index * 8;

        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);

    }
}
