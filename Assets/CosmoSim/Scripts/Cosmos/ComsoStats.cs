using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComsoStats : MonoBehaviour
{
	[System.Serializable]
	public class ValueRange
	{
		public float percent;
		public Vector2 numPlanets = new Vector2(2f, 8f);
	}

	public class ElementAbundance
	{
		public int element;
		public float percent;		
	}

	//Percentage og starts having a planetary sistem
	float starsWithPlanets = 0.9f; // 9 in 10
	//Number of planest in the system
	public List<ValueRange> planets = new List<ValueRange>();

	//Probability of having an asteroids belt in the star system
	public float asteroidsBelt = 0.01f; //1 in 100

	//Probability of a start being a black hole
	public float blackHole = 0.0001f; // 1 in 10000
	public List<ValueRange> blackHoleSize = new List<ValueRange>();

	//Abundance on elements in percentage
	public List<string> element = new List<string>();

	//Default abundance of elements in the cosmos
	public List<ElementAbundance> cosmos = new List<ElementAbundance>();

	//Abundance of elements in stars
	public List<ElementAbundance> regularStar = new List<ElementAbundance>();

	//Abundance of elements in telutic planets
	public List<ElementAbundance> teluricPlanet = new List<ElementAbundance>();

	//Abundance of elements in giant gas planets
	public List<ElementAbundance> gasGiantPlanet = new List<ElementAbundance>();
}
