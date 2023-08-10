using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour {

	public List<SpectralClass> spectralClass = new List<SpectralClass>();
	public List<SpectralClass> spectralClassBackup = new List<SpectralClass>();

	public float ratioOfStarsWithPlanets = 0.8f;	//80% starts has planets
	public static Physics instance;

	void Awake()
	{
		instance = this;
	}

	public int PositionInArray(char spClass)
	{
		for (int i = 0; i < spectralClass.Count; i++)
			if (spClass == spectralClass[i].Type[0])
				return i;
		return spectralClass.Count;
	}
}
