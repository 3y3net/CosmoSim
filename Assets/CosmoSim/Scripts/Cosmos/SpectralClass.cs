using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpectralClass {
	
	[System.Serializable]
	public class ProbabilityDistribution
	{
		public float percent;
		public Vector2 ValueRange = new Vector2(2f, 8f);

	}

	public string Type;
	public float percent;
	public Color color;
	public Vector2 luminosityRange = new Vector2(0f, 16f);
	public List<ProbabilityDistribution> luminosity = new List<ProbabilityDistribution>();
	public Vector2 MassRange = new Vector2(0f, 16f);
	public Vector2 RadiusRange = new Vector2(0f, 16f);
	public Vector2 TemperatureRange = new Vector2(0f, 16f);
	public Vector2Int PlanetRange = new Vector2Int(0, 22);
	public List<ProbabilityDistribution> planets = new List<ProbabilityDistribution>();
	public int gridSectorSize;
	public int gridCenterSector;
	public Transform model;
	
}
