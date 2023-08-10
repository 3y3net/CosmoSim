using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public string planetName, planetType; // planet type would be like: Gas Giant, Dwarf Planet, Hot Terra
    public float mass, radius, gravity, atmoPressure, temperature, distFromStar;
    public int numOfSattelites; // Or moons if you care to call them that
    public bool life;
    public string lifeType; // Multicellular, Singlecelled, Complex Aquatic, Complex terestrial, Sapeint

	private void Start () {
		
	}	
}