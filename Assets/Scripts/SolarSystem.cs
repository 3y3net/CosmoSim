using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour {

    public Star star;
    public string starName;
    public char starClass; // M K G F A
    public int primaryPlanetNum = 0; // Number of planets that directly orbit the star(s)
    public float starRadius = 0; // in km
    public int seed; // Random number seed to keep this solar system looking the same every time we enter.
    public GameObject starM, starK, starG, starF, starA; // prefabs of different star classes
    public GameObject planet;

    public void InitSolarSystem(Star s)
    {
        ClearSolarSystem();

        star = s;

        if (star != null)
        {
            starClass = star.spectralClass[0];
            primaryPlanetNum = star.numberOfPlanets;
            starRadius = (700000 * star.radius);
        }
        Random.InitState(seed + starClass + primaryPlanetNum);
        GameObject detailedStar = null;
        // Instantiate our star
        if (starClass == 'M')
        {
            detailedStar = Instantiate(starM, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
            if (starRadius == 0)
                starRadius = 0.15f * 700000f;
        }
        else if (starClass == 'K')
        {
            detailedStar = Instantiate(starK, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
            if (starRadius == 0)
                starRadius = 0.75f * 700000f;
        }
        else if (starClass == 'G')
        {
            detailedStar = Instantiate(starG, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
            if (starRadius == 0)
                starRadius = 1f * 700000f;
        }
        else if (starClass == 'F')
        {
            detailedStar = Instantiate(starF, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
            if (starRadius == 0)
                starRadius = 1.2f * 700000f;
        }
        else if (starClass == 'A')
        {
            detailedStar = Instantiate(starA, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
            if (starRadius == 0)
                starRadius = 1.5f * 700000f;
        }
        else
        {
            print("Error: Solar System 'starClass' does not match any known star Classification: " + starClass + " Correct starClass Values: M K G F A");
        }
        if (detailedStar != null)
        {
            detailedStar.transform.localScale = new Vector3(starRadius * 0.00001f, starRadius * 0.00001f, starRadius * 0.00001f);
        }
        // Instantiate our random planets
        for (int i = 0; i < primaryPlanetNum; i++)
        {
            GeneratePlanet();
        }
    }

    // Generates one random planet.
    private void GeneratePlanet()
    {
        /* All the variables we have to generate for our new planet
        public string planetName, planetType; // planet type would be like: Gas Giant, Dwarf Planet, Hot Terra
        public float mass, radius, gravity, atmoPressure, temperature, distFromStar;
        public int numOfSattelites; // Or moons if you care to call them that
        public bool life;
        public string lifeType; // Multicellular, Singlecelled, Complex Aquatic, Complex terestrial, Sapeint
        */
        
        GameObject newPlanetBaryCenter = Instantiate(planet, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), transform);
        Transform newPlanet = newPlanetBaryCenter.transform.GetChild(0); // get the actual planet
        // random distance from star in AU
        float auFromStar = Random.Range(0.4f, 40.0f); // this needs a lot of adjustments in the future
        newPlanet.localPosition = new Vector3(auFromStar * 10f, 0, 0);
        Planet pData = newPlanet.GetComponent<Planet>(); // planet script of the planet
        pData.distFromStar = auFromStar;
        // random rotation of the planets parent so the planet appears to be in a random position around its star
        float orbitRotation = Random.Range(-180f, 180f);
        newPlanetBaryCenter.transform.Rotate(0, orbitRotation, 0);
        float radiusOfPlanet = Random.Range(1000f, 80000); // in km
        pData.radius = radiusOfPlanet;
        newPlanet.transform.localScale = new Vector3(radiusOfPlanet * 0.00001f, radiusOfPlanet * 0.00001f, radiusOfPlanet * 0.00001f);
    }

    public void ClearSolarSystem()
    {
        if (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
