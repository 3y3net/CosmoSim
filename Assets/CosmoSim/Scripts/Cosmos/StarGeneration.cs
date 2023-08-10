
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StarGeneration : MonoBehaviour
{

    public float maxStarSize = 1.25f;
    public GameObject starPrefab;    
    private GameObject starContainer; // The parents of the stars and galactic dust particles
    private List<Transform> poolStars; // this is where we will store all star GameObjects until we need more stars
    public Galaxy galaxy; // The galaxy that we are near or inside of
    // a group of arrays of Lists for each class of stars representing each sector
    private List<Transform>[][,,] sectorStars;
    
    // the bounding boxes for each star in each sector to detect clicking because giving stars colliders resulted in terrible performance
    private List<Bounds>[][,,] sectorStarBounds;
    // Very important! These cubed are the sizes of the star grids of the different classes.
    
    private float gSectDiameterH; // horizontal diameter of 1 sector of the grid
    private float gSectDiameterV; // vertical diameter of 1 sector of the grid
    // Position of each respective sector in the galaxy grid to be generated
    private Vector3Int[][,,] starSectorGridPositions;
    private Vector3Int lastCenterSectorGridPos; // This is the last location the camera was in thus the center of our sector grid

    private bool hasGeneratedFirstStars = false;
    //private bool galaxyGenComplete = false; Have all of the galaxies generated yet?

    private float timeSincePrintStarPoolStats = 0f;
    private int mostStarsUsedAtOnce = 0;

    private Transform starCam;
    private int cToScale = 0; // Scale and rotate only one class of stars every frame
    private bool adjustedNearStars = false; // have we scaled and rotated nearby stars to match new camera location yet
    private bool adjustedFarStars = false; // have we scaled and rotated far away stars to match new camera location yet
    private Vector3 lastDistantStarUpdatePos; // last position the starCam was when the far away stars were updated
    private Vector3 lastCenterStarUpdatePos; // last cam pos when the center stars updated

    public bool debugLines = false;
    public int specClassDebug = 5;

    private Random.State randomStates;
    private bool randomStarted = false;

    public List<bool> maskSpectral = new List<bool>();
    public bool databaseCreated = false;

    void Start()
    {
        gSectDiameterH = galaxy.sectorSize;
        gSectDiameterV = galaxy.sectorSize;

        starContainer = GameObject.Find("StarContainer");
        starCam = GameObject.FindObjectOfType<StarCamera>().transform;
        lastDistantStarUpdatePos = starCam.position;
        lastCenterStarUpdatePos = starCam.position;

        //for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)

        sectorStars = new List<Transform>[Physics.instance.spectralClass.Count][,,];
        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            sectorStars[ec] = new List<Transform>[Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize];

        starSectorGridPositions = new Vector3Int[Physics.instance.spectralClass.Count][,,];
        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            starSectorGridPositions[ec] = new Vector3Int[Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize];

        sectorStarBounds = new List<Bounds>[Physics.instance.spectralClass.Count][,,];
        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            sectorStarBounds[ec] = new List<Bounds>[Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize, Physics.instance.spectralClass[ec].gridSectorSize];

        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {
            for (int i = 0; i < Physics.instance.spectralClass[ec].gridSectorSize; i++)
            {
                for (int j = 0; j < Physics.instance.spectralClass[ec].gridSectorSize; j++)
                {
                    for (int k = 0; k < Physics.instance.spectralClass[ec].gridSectorSize; k++)
                    {
                        sectorStars[ec][i, j, k] = new List<Transform>();
                        starSectorGridPositions[ec][i, j, k] = new Vector3Int();
                        sectorStarBounds[ec][i, j, k] = new List<Bounds>();
                    }
                }
            }
        }


        // Here we populate our Star Pool with stars for later use. Note that every pixel in our galaxy image can hold only a maximum of starResolutions
        // value of stars so we only have to populate the pool for the x and z cordinates in our grid
        int numOfStarsInPool = 40000;
        poolStars = new List<Transform>(); // this is where all of the stars not in use should be placed
        for (int s = 0; s < numOfStarsInPool; s++)
        {
            GameObject star = Instantiate(starPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), starContainer.transform);
            star.transform.GetComponent<Star>().mat = star.transform.GetComponent<SpriteRenderer>().material;
            poolStars.Add(star.transform);
        }
        
    }

    

    public void Update()
    {
        if (galaxy.galaxyGenComplete)
        {
            //if(!databaseCreated)
            //    CreateStarDB();

            ManageStars();
            // scale and rotate our stars and bounds
            if (Vector3.Distance(lastDistantStarUpdatePos, starCam.position) > 5f)
            {
                adjustedFarStars = false;
                lastDistantStarUpdatePos = starCam.position;
            }
            else if (Vector3.Distance(lastCenterStarUpdatePos, starCam.position) > 0.01f)
            {
                adjustedNearStars = false;
                lastCenterStarUpdatePos = starCam.position;
            }
            if (!adjustedFarStars)
                AdjustAllStarSectors(false);
            else if (!adjustedNearStars)
                AdjustAllStarSectors(true);
            
        }

        if (timeSincePrintStarPoolStats >= 2f)
        {
            int numOfStarsInUse = 0;
            timeSincePrintStarPoolStats = 0f;

            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {                
                for (int i = 0; i < Physics.instance.spectralClass[ec].gridSectorSize; i++)
                {
                    for (int j = 0; j < Physics.instance.spectralClass[ec].gridSectorSize; j++)
                    {
                        for (int k = 0; k < Physics.instance.spectralClass[ec].gridSectorSize; k++)
                        {
                            numOfStarsInUse += sectorStars[ec][i, j, k].Count;
                        }
                    }
                }
            }

            if (numOfStarsInUse > mostStarsUsedAtOnce) mostStarsUsedAtOnce = numOfStarsInUse;

            //print("Num Of stars in pool: " + poolStars.Count + " Num of stars in use " + numOfStarsInUse + " Most Used: " + mostStarsUsedAtOnce + " Total: " + (poolStars.Count + numOfStarsInUse));
        }
        timeSincePrintStarPoolStats += Time.deltaTime;
    }

    // Move stars along the grid Sectors and call to generate new ones in new grid locations moving forward and so on
    private void ManageStars()
    {
        randomStarted = false;
        Vector3 camPos = starCam.position;

        Vector3Int centerGridLoc = new Vector3Int(); // Location of the star grid sector that the camera is in
        centerGridLoc.x = (int)((((camPos.x - (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
        centerGridLoc.y = (int)(((((camPos.y + (gSectDiameterV / 2)) * -1) / galaxy.radiusV) + 1) * (galaxy.gridDiameterV / 2));
        centerGridLoc.z = (int)((((camPos.z + (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));

        Vector3Int differenceInLoc = centerGridLoc - lastCenterSectorGridPos;
        if (centerGridLoc != lastCenterSectorGridPos) // if we have moved to a new grid location
        {
            lastCenterSectorGridPos = centerGridLoc;
        }

        // move grid on X
        if ((differenceInLoc.x == 1 || differenceInLoc.x == -1) && hasGeneratedFirstStars)
        {


            int PorN = differenceInLoc.x;

            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                int xStart = 0, xEnd = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                if (PorN == -1)
                {
                    xStart = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                    xEnd = 0;
                }
                for (int y = 0; y < Physics.instance.spectralClass[ec].gridSectorSize; y++)
                {
                    for (int z = 0; z < Physics.instance.spectralClass[ec].gridSectorSize; z++)
                    {
                        List<Transform> tempStarHolster = sectorStars[ec][xStart, y, z];
                        List<Bounds> tempBoundsHolster = sectorStarBounds[ec][xStart, y, z];
                        for (int x = xStart; (x < Physics.instance.spectralClass[ec].gridSectorSize) && (x >= 0); x += PorN)
                        {
                            if (x == xEnd)
                            {
                                sectorStars[ec][x, y, z] = tempStarHolster;
                                sectorStarBounds[ec][x, y, z] = tempBoundsHolster;
                                Vector3 sectorPos;
                                sectorPos.x = (gSectDiameterH * (x - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.x;
                                sectorPos.y = (gSectDiameterV * (y - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.y;
                                sectorPos.z = (gSectDiameterH * (z - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.z;

                                starSectorGridPositions[ec][x, y, z].x = (int)((((sectorPos.x - (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                starSectorGridPositions[ec][x, y, z].y = (int)(((((sectorPos.y + (gSectDiameterV / 2)) * -1) / galaxy.radiusV) + 1) * (galaxy.gridDiameterV / 2));
                                starSectorGridPositions[ec][x, y, z].z = (int)((((sectorPos.z + (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                GenerateStarsInArea(starSectorGridPositions[ec][x, y, z], new Vector3Int(x, y, z), Physics.instance.spectralClass[ec].Type[0]);
                            }
                            else
                            {
                                starSectorGridPositions[ec][x, y, z] = starSectorGridPositions[ec][x + PorN, y, z];
                                sectorStars[ec][x, y, z] = sectorStars[ec][x + PorN, y, z];
                                sectorStarBounds[ec][x, y, z] = sectorStarBounds[ec][x + PorN, y, z];
                            }
                        }
                    }
                }

            }
        }
        // move grid on Z
        if ((differenceInLoc.z == 1 || differenceInLoc.z == -1) && hasGeneratedFirstStars)
        {
            int PorN = differenceInLoc.z;
            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                int zStart = 0, zEnd = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                if (PorN == -1)
                {
                    zStart = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                    zEnd = 0;
                }
                for (int x = 0; x < Physics.instance.spectralClass[ec].gridSectorSize; x++)
                {
                    for (int y = 0; y < Physics.instance.spectralClass[ec].gridSectorSize; y++)
                    {
                        List<Transform> tempStarHolster = sectorStars[ec][x, y, zStart];
                        List<Bounds> tempBoundsHolster = sectorStarBounds[ec][x, y, zStart];
                        for (int z = zStart; (z < Physics.instance.spectralClass[ec].gridSectorSize) && (z >= 0); z += PorN)
                        {
                            if (z == zEnd)
                            {
                                sectorStars[ec][x, y, z] = tempStarHolster;
                                sectorStarBounds[ec][x, y, z] = tempBoundsHolster;

                                Vector3 sectorPos;
                                sectorPos.x = (gSectDiameterH * (x - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.x;
                                sectorPos.y = (gSectDiameterV * (y - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.y;
                                sectorPos.z = (gSectDiameterH * (z - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.z;

                                starSectorGridPositions[ec][x, y, z].x = (int)((((sectorPos.x - (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                starSectorGridPositions[ec][x, y, z].y = (int)(((((sectorPos.y + (gSectDiameterV / 2)) * -1) / galaxy.radiusV) + 1) * (galaxy.gridDiameterV / 2));
                                starSectorGridPositions[ec][x, y, z].z = (int)((((sectorPos.z + (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                GenerateStarsInArea(starSectorGridPositions[ec][x, y, z], new Vector3Int(x, y, z), Physics.instance.spectralClass[ec].Type[0]);
                            }
                            else
                            {
                                starSectorGridPositions[ec][x, y, z] = starSectorGridPositions[ec][x, y, z + PorN];
                                sectorStars[ec][x, y, z] = sectorStars[ec][x, y, z + PorN];
                                sectorStarBounds[ec][x, y, z] = sectorStarBounds[ec][x, y, z + PorN];
                            }
                        }
                    }
                }
            }

        }
        // move on y
        if ((differenceInLoc.y == 1 || differenceInLoc.y == -1) && hasGeneratedFirstStars)
        {
            int PorN = differenceInLoc.y * -1;

            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                int yStart = 0, yEnd = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                if (PorN == -1)
                {
                    yStart = Physics.instance.spectralClass[ec].gridSectorSize - 1;
                    yEnd = 0;
                }
                for (int x = 0; x < Physics.instance.spectralClass[ec].gridSectorSize; x++)
                {
                    for (int z = 0; z < Physics.instance.spectralClass[ec].gridSectorSize; z++)
                    {
                        List<Transform> tempStarHolster = sectorStars[ec][x, yStart, z];
                        List<Bounds> tempBoundsHolster = sectorStarBounds[ec][x, yStart, z];
                        for (int y = yStart; (y < Physics.instance.spectralClass[ec].gridSectorSize) && (y >= 0); y += PorN)
                        {
                            if (y == yEnd)
                            {
                                sectorStars[ec][x, y, z] = tempStarHolster;
                                sectorStarBounds[ec][x, y, z] = tempBoundsHolster;

                                Vector3 sectorPos;
                                sectorPos.x = (gSectDiameterH * (x - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.x;
                                sectorPos.y = (gSectDiameterV * (y - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.y;
                                sectorPos.z = (gSectDiameterH * (z - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.z;

                                starSectorGridPositions[ec][x, y, z].x = (int)((((sectorPos.x - (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                starSectorGridPositions[ec][x, y, z].y = (int)(((((sectorPos.y + (gSectDiameterV / 2)) * -1) / galaxy.radiusV) + 1) * (galaxy.gridDiameterV / 2));
                                starSectorGridPositions[ec][x, y, z].z = (int)((((sectorPos.z + (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                                GenerateStarsInArea(starSectorGridPositions[ec][x, y, z], new Vector3Int(x, y, z), Physics.instance.spectralClass[ec].Type[0]);
                            }
                            else
                            {
                                starSectorGridPositions[ec][x, y, z] = starSectorGridPositions[ec][x, y + PorN, z];
                                sectorStars[ec][x, y, z] = sectorStars[ec][x, y + PorN, z];
                                sectorStarBounds[ec][x, y, z] = sectorStarBounds[ec][x, y + PorN, z];
                            }
                        }
                    }
                }
            }

        }

        if ((differenceInLoc.x > 1 || differenceInLoc.x < -1) || (differenceInLoc.y > 1 || differenceInLoc.y < -1) || (differenceInLoc.z > 1 || differenceInLoc.z < -1))
        {
            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                //print("Updating All Star Sectors: " + differenceInLoc);
                for (int x = 0; x < Physics.instance.spectralClass[ec].gridSectorSize; x++)
                {
                    for (int y = 0; y < Physics.instance.spectralClass[ec].gridSectorSize; y++)
                    {
                        for (int z = 0; z < Physics.instance.spectralClass[ec].gridSectorSize; z++)
                        {
                            Vector3 sectorPos;
                            sectorPos.x = (gSectDiameterH * (x - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.x;
                            sectorPos.y = (gSectDiameterV * (y - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.y;
                            sectorPos.z = (gSectDiameterH * (z - Physics.instance.spectralClass[ec].gridCenterSector)) + camPos.z;

                            starSectorGridPositions[ec][x, y, z].x = (int)((((sectorPos.x - (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                            starSectorGridPositions[ec][x, y, z].y = (int)(((((sectorPos.y + (gSectDiameterV / 2)) * -1) / galaxy.radiusV) + 1) * (galaxy.gridDiameterV / 2));
                            starSectorGridPositions[ec][x, y, z].z = (int)((((sectorPos.z + (gSectDiameterH / 2)) / galaxy.radiusH) + 1) * (galaxy.gridDiameterH / 2));
                            GenerateStarsInArea(starSectorGridPositions[ec][x, y, z], new Vector3Int(x, y, z), Physics.instance.spectralClass[ec].Type[0]);
                        }
                    }
                }
            }
            
            hasGeneratedFirstStars = true;
        }

        if (debugLines)
        {
           
            for (int x = 0; x < Physics.instance.spectralClass[specClassDebug].gridSectorSize; x++)
            {
                for (int y = 0; y < Physics.instance.spectralClass[specClassDebug].gridSectorSize; y++)
                {
                    for (int z = 0; z < Physics.instance.spectralClass[specClassDebug].gridSectorSize; z++)
                    {
                        float areaStartX = (-galaxy.radiusH + gSectDiameterH * (starSectorGridPositions[specClassDebug][x, y, z].x + 1)) - (gSectDiameterH / 2);
                        float areaEndX = areaStartX + gSectDiameterH;
                        float areaStartY = (galaxy.radiusV - (gSectDiameterV * (starSectorGridPositions[specClassDebug][x, y, z].y))) - (gSectDiameterV / 2);
                        float areaEndY = areaStartY - gSectDiameterV;
                        float areaStartZ = (-galaxy.radiusH + (gSectDiameterH * (starSectorGridPositions[specClassDebug][x, y, z].z + 1))) - (gSectDiameterH / 2);
                        float areaEndZ = areaStartZ - gSectDiameterH;

                        Vector3 v0 = new Vector3(areaStartX, areaStartY, areaStartZ);
                        Vector3 v1 = new Vector3(areaEndX, areaStartY, areaStartZ);
                        Vector3 v2 = new Vector3(areaStartX, areaStartY, areaEndZ);
                        Vector3 v3 = new Vector3(areaEndX, areaStartY, areaEndZ);
                        Vector3 v4 = new Vector3(areaStartX, areaEndY, areaStartZ);
                        Vector3 v5 = new Vector3(areaEndX, areaEndY, areaStartZ);
                        Vector3 v6 = new Vector3(areaStartX, areaEndY, areaEndZ);
                        Vector3 v7 = new Vector3(areaEndX, areaEndY, areaEndZ);

                        Debug.DrawLine(v0, v1, Color.yellow, 0);
                        Debug.DrawLine(v0, v2, Color.yellow, 0);
                        Debug.DrawLine(v2, v3, Color.yellow, 0);
                        Debug.DrawLine(v3, v1, Color.yellow, 0);

                        Debug.DrawLine(v4, v5, Color.yellow, 0);
                        Debug.DrawLine(v4, v6, Color.yellow, 0);
                        Debug.DrawLine(v6, v7, Color.yellow, 0);
                        Debug.DrawLine(v7, v5, Color.yellow, 0);

                        Debug.DrawLine(v0, v4, Color.yellow, 0);
                        Debug.DrawLine(v2, v6, Color.yellow, 0);
                        Debug.DrawLine(v1, v5, Color.yellow, 0);
                        Debug.DrawLine(v3, v7, Color.yellow, 0);
                    }
                }
            }
        }
    }

    // generates stars in a given location of the grid to a given parent
    private void GenerateStarsInArea(Vector3Int gridLoc, Vector3Int sectorID, char starClass)
    {
        if (gridLoc.x > galaxy.gridDiameterH - 1 || gridLoc.x <= 0 || gridLoc.y > galaxy.gridDiameterV - 1 || gridLoc.y <= 0 || gridLoc.z > galaxy.gridDiameterH - 1 || gridLoc.z <= 0)
        {
            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                // If we are outside of the galaxy grid then it's safe to assume there are no stars so throw all the ones in use back into the pool
                if (starClass == Physics.instance.spectralClass[ec].Type[0])
                {
                    if (sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count != 0)
                    {
                        for (int i = sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count - 1; i >= 0; i--)
                        {
                            sectorStars[ec][sectorID.x, sectorID.y, sectorID.z][i].gameObject.SetActive(false);
                            poolStars.Add(sectorStars[ec][sectorID.x, sectorID.y, sectorID.z][i]);
                            sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].RemoveAt(i);
                        }
                    }
                }
            }
        }
        else
        {   // lets generate some stars now
            // determine the chunk of the area this grid location represents and the number of stars that should exist here
            float areaStartX = (-galaxy.radiusH + gSectDiameterH * (gridLoc.x + 1)) - (gSectDiameterH / 2);
            float areaEndX = areaStartX + gSectDiameterH;
            float areaStartY = (galaxy.radiusV - (gSectDiameterV * (gridLoc.y))) - (gSectDiameterV / 2);
            float areaEndY = areaStartY - gSectDiameterV;
            float areaStartZ = (-galaxy.radiusH + (gSectDiameterH * (gridLoc.z + 1))) - (gSectDiameterH / 2);
            float areaEndZ = areaStartZ - gSectDiameterH;

            float probFloat = 0f; // Use this guy to store probabilities.

            // if star class counts have not been populated for this grid location then do it now
            int totalStartInSector = 0;
            for (int i = 0; i < Physics.instance.spectralClass.Count; i++)
                totalStartInSector += galaxy.starPopGridSpectralClass[i, gridLoc.x, gridLoc.z, gridLoc.y];
            if (totalStartInSector != galaxy.starPopGridTotal[gridLoc.x, gridLoc.z, gridLoc.y])
            {
                int currentSeed = galaxy.seed + gridLoc.x + (gridLoc.y * 1000) + (gridLoc.z * 1000000);
                Random.InitState(currentSeed);



                // randomly calculate the classifications of the stars in this sector
                for (int s = 0; s < galaxy.starPopGridTotal[gridLoc.x, gridLoc.z, gridLoc.y]; s++)
                {
                    float randStarClassNum = Random.Range(0f, 100f);
                    float percent = 0;
                    for (int i = 0; i < Physics.instance.spectralClass.Count; i++)
                    {                        
                        percent += Physics.instance.spectralClass[i].percent;
                        if (randStarClassNum <= percent)
                        {
                            galaxy.starPopGridSpectralClass[i, gridLoc.x, gridLoc.z, gridLoc.y]++;
                            break;
                        }
                    }
                }
            }

            int starCount = 0;
            for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
            {
                if (!maskSpectral[ec])
                    continue;
                if (starClass == Physics.instance.spectralClass[ec].Type[0])
                {
                    int numOfStarsToMakeM = galaxy.starPopGridSpectralClass[ec, gridLoc.x, gridLoc.z, gridLoc.y];
                    int newSeed = galaxy.seed + (numOfStarsToMakeM * starClass) + gridLoc.x + (gridLoc.y * 1000) + (gridLoc.z * 1000000);
                    Random.InitState(newSeed);

                    // Set up the right number of stars
                    //Debug.Log(":"+ec+" - "+sectorID);
                    //Si hay de más, desactiva y manda al pool
                    if (sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count > numOfStarsToMakeM)
                    {
                        for (int i = sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count - 1; i > numOfStarsToMakeM - 1; i--)
                        {
                            sectorStars[ec][sectorID.x, sectorID.y, sectorID.z][i].gameObject.SetActive(false);
                            poolStars.Add(sectorStars[ec][sectorID.x, sectorID.y, sectorID.z][i]);
                            sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].RemoveAt(i);
                        }
                    }
                    //Si faltan coge del pool
                    else if (sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count < numOfStarsToMakeM)
                    {
                        for (int i = sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count - 1; i < numOfStarsToMakeM - 1; i++)
                        {
                            sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Add(poolStars[poolStars.Count - 1]);
                            poolStars.RemoveAt(poolStars.Count - 1);
                        }
                    }
                    sectorStarBounds[ec][sectorID.x, sectorID.y, sectorID.z].Clear();

                    // Procedurally Generate Stars
                    for (int i = 0; i < sectorStars[ec][sectorID.x, sectorID.y, sectorID.z].Count; i++)
                    {
                        Transform star = sectorStars[ec][sectorID.x, sectorID.y, sectorID.z][i];
                        star.gameObject.SetActive(true);
                        star.position = new Vector3(Random.Range(areaStartX, areaEndX), Random.Range(areaStartY, areaEndY), Random.Range(areaStartZ, areaEndZ));
                        sectorStarBounds[ec][sectorID.x, sectorID.y, sectorID.z].Add(new Bounds(star.position, new Vector3(0.15f, 0.15f, 0.15f)));
                        Star starScript = star.GetComponent<Star>(); //get the star script atached to this star prefab so we can set some properties
                        
                        starScript.starSeed= newSeed + starCount++;

                        /*
                        starScript.spectralClass = Physics.instance.spectralClass[ec].Type;
                        
                        float percent = 0;
                        probFloat = Random.Range(0f, 100f);

                        for (int lum = 0; lum < Physics.instance.spectralClass[ec].luminosity.Count; lum++)
                        {
                            percent += Physics.instance.spectralClass[ec].luminosity[lum].percent;

                            if (probFloat < percent)
                            {
                                starScript.luminosity = Random.Range(Physics.instance.spectralClass[ec].luminosity[lum].ValueRange.x, Physics.instance.spectralClass[ec].luminosity[lum].ValueRange.y);
                                break;
                            }
                        }

                        bool hasPlanets = Random.Range(0f, 1f) < Physics.instance.ratioOfStarsWithPlanets;
                        if (hasPlanets)
                        {
                            percent = 0;
                            probFloat = Random.Range(0f, 100f);

                            for (int planets = 0; planets < Physics.instance.spectralClass[ec].planets.Count; planets++)
                            {
                                percent += Physics.instance.spectralClass[ec].planets[planets].percent;

                                if (probFloat < percent)
                                {
                                    starScript.numberOfPlanets = Random.Range((int)Physics.instance.spectralClass[ec].planets[planets].ValueRange.x, (int)Physics.instance.spectralClass[ec].planets[planets].ValueRange.y);
                                    break;
                                }
                            }
                        }
                        else
                            starScript.numberOfPlanets = 0;

                        */
                        //starScript.PrepStar();

                        starScript.GenerateInfo(Physics.instance.spectralClass[ec].Type);

                        starScript.mat.color = new Color(Physics.instance.spectralClass[ec].color.r, Physics.instance.spectralClass[ec].color.g, Physics.instance.spectralClass[ec].color.b);
                        starScript.ResizeStar(starCam.position, false);
                        star.LookAt(starCam);
                        
                    }
                }
            }

        }
    }


    public void GenerateStar(char starClass)
	{

	}

    // Check to see if ray has hit one of the star bounds and if so return that stars transform
    // we should definetly optimize this to only check the sectors within the cameras view or the ones that the ray actually passes through only
    public bool RaycastToStar(Ray ray, out Transform star)
    {
        star = null;
        int index = -1;
        float min = float.MaxValue;
        float current = 0;

        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {

            for (int i = 0; i < Physics.instance.spectralClass[ec].gridSectorSize; i++)
            {
                for (int j = 0; j < Physics.instance.spectralClass[ec].gridSectorSize; j++)
                {
                    for (int k = 0; k < Physics.instance.spectralClass[ec].gridSectorSize; k++)
                    {
                        for (int b = 0; b < sectorStarBounds[ec][i, j, k].Count; b++)
                        {
                            if (sectorStarBounds[ec][i, j, k][b].IntersectRay(ray, out current) && current < min)
                            {
                                index = i;
                                min = current;
                                star = sectorStars[ec][i, j, k][b];                                
                                //star.GetComponent<Star>().GenerateInfo(starSectorGridPositions[ec][i, j, k], b);
                            }
                        }
                    }
                }
            }
        }


        return index >= 0;
    }

    // Check how close stars in center grid location are to a position(most likely the star/galaxy camera) and return any star that is very close to that position.
    // 1582AU == 0.025LY
    public bool CloseStar(out GameObject closeStar, Vector3 pos, float distance)
    {
        closeStar = null;
        bool foundOne = false;

        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {
            for (int s = 0; s < sectorStars[ec][Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector].Count; s++)
            {
                if (Vector3.Distance(sectorStars[ec][Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector][s].position, pos) < distance)
                {
                    closeStar = sectorStars[ec][Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector, Physics.instance.spectralClass[ec].gridCenterSector][s].gameObject;
                    int gc = Physics.instance.spectralClass[ec].gridCenterSector;
                    //closeStar.transform.GetComponent<Star>().GenerateInfo(starSectorGridPositions[ec][gc, gc, gc], s);
                    foundOne = true;
                    break;
                }
            }
            if (foundOne)
                break;
        }

        
        return foundOne;
    }

    // As we move through space stars will be rotated and scaled properly along with thier bounds but more often as they get closer
    // If close only is true then we iterate through only stars within 3 blocks of center grid
    // THIS IS NOT OPTIMIZED TO AN ACCEPTABLE DEGREE!!! 
    private void AdjustAllStarSectors(bool closeOnly)
    {
        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {

            if (cToScale == ec)
            {
                cToScale = (ec+1)% Physics.instance.spectralClass.Count;
                // It doesn't matter if closeOnly is true or not for M Class stars they will always be updated for now
                for (int i = 0; i < Physics.instance.spectralClass[ec].gridSectorSize; i++)
                {
                    for (int j = 0; j < Physics.instance.spectralClass[ec].gridSectorSize; j++)
                    {
                        for (int k = 0; k < Physics.instance.spectralClass[ec].gridSectorSize; k++)
                        {
                            for (int s = 0; s < sectorStars[ec][i, j, k].Count; s++)
                            {
                                float scaleB = sectorStars[ec][i, j, k][s].GetComponent<Star>().ResizeStar(starCam.position, false);
                                sectorStarBounds[ec][i, j, k][s] = new Bounds(sectorStars[ec][i, j, k][s].position, new Vector3(scaleB, scaleB, scaleB));
                                sectorStars[ec][i, j, k][s].LookAt(starCam);
                            }
                        }
                    }
                }
                break;
            }
        }
        
    }    


    public void CreateStarDB()
    {
        databaseCreated = true;
        //Path of the file.
        string path = Application.dataPath + "/StarDatabase. txt";
        Debug.Log(path);

        StreamWriter sw;
        //Create File if it doesn't exist.
        if (!File.Exists(path))
        {
            // Create a file to write to.
            sw = File.CreateText(path);
            sw.WriteLine("");
        }
        else
        {
            sw = File.AppendText(path);
            sw.WriteLine("NEW APPEND");
        }

        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {
            for (int x = 0; x < galaxy.gridDiameterH; x++)
            {
                for (int y = 0; y < galaxy.gridDiameterV; y++)
                {
                    for (int z = 0; z < galaxy.gridDiameterH; z++)
                    {
                        GenerateStarsInAreaForDB(new Vector3Int(x, y, z), new Vector3Int(0, 0, 0), Physics.instance.spectralClass[ec].Type[0], sw);
                    }
                }
            }
        }

        sw.Close();
    }

    private void GenerateStarsInAreaForDB(Vector3Int gridLoc, Vector3Int sectorID, char starClass, StreamWriter sw)
    {
        // lets generate some stars now
        // determine the chunk of the area this grid location represents and the number of stars that should exist here
        float areaStartX = (-galaxy.radiusH + gSectDiameterH * (gridLoc.x + 1)) - (gSectDiameterH / 2);
        float areaEndX = areaStartX + gSectDiameterH;
        float areaStartY = (galaxy.radiusV - (gSectDiameterV * (gridLoc.y))) - (gSectDiameterV / 2);
        float areaEndY = areaStartY - gSectDiameterV;
        float areaStartZ = (-galaxy.radiusH + (gSectDiameterH * (gridLoc.z + 1))) - (gSectDiameterH / 2);
        float areaEndZ = areaStartZ - gSectDiameterH;

        Debug.Log(gridLoc);
        // if star class counts have not been populated for this grid location then do it now
        int totalStartInSector = 0;
        for (int i = 0; i < Physics.instance.spectralClass.Count; i++)
            totalStartInSector += galaxy.starPopGridSpectralClass[i, gridLoc.x, gridLoc.z, gridLoc.y];
        if (totalStartInSector != galaxy.starPopGridTotal[gridLoc.x, gridLoc.z, gridLoc.y])
        {
            int currentSeed = galaxy.seed + gridLoc.x + (gridLoc.y * 1000) + (gridLoc.z * 1000000);
            Random.InitState(currentSeed);



            // randomly calculate the classifications of the stars in this sector
            for (int s = 0; s < galaxy.starPopGridTotal[gridLoc.x, gridLoc.z, gridLoc.y]; s++)
            {
                float randStarClassNum = Random.Range(0f, 100f);
                float percent = 0;
                for (int i = 0; i < Physics.instance.spectralClass.Count; i++)
                {
                    percent += Physics.instance.spectralClass[i].percent;
                    if (randStarClassNum <= percent)
                    {
                        galaxy.starPopGridSpectralClass[i, gridLoc.x, gridLoc.z, gridLoc.y]++;
                        break;
                    }
                }
            }
        }

        int starCount = 0;
        for (int ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {
                
            if (starClass == Physics.instance.spectralClass[ec].Type[0])
            {
                int numOfStarsToMakeM = galaxy.starPopGridSpectralClass[ec, gridLoc.x, gridLoc.z, gridLoc.y];
                int newSeed = galaxy.seed + (numOfStarsToMakeM * starClass) + gridLoc.x + (gridLoc.y * 1000) + (gridLoc.z * 1000000);
                Random.InitState(newSeed);
                
                // Procedurally Generate Stars
                for (int i = 0; i < numOfStarsToMakeM; i++)
                {
                    Vector3 position = new Vector3(Random.Range(areaStartX, areaEndX), Random.Range(areaStartY, areaEndY), Random.Range(areaStartZ, areaEndZ));
                    int starSeed = newSeed + starCount++;

                    sw.WriteLine(starSeed+", "+ position.x+ ", " + position.y + ", " + position.z);

                }
            }
        }
    }


}