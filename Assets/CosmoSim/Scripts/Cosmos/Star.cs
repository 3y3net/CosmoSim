using UnityEngine;

public class Star : MonoBehaviour
{
    public string starName, spectralClass;
    public float radius, luminosity;
    public int temperature, numberOfPlanets;
    public Color color;
    public bool hasLife;
    public Material mat;
    public int starSeed = 0;
    
    public float ResizeStar(Vector3 camPos, bool printBright)
    {
        float dist = Vector3.Distance(transform.position, camPos);
        float bright = (luminosity / (4f * 3.14f * (dist * dist))) * 190f;
        if (bright > 0.005f) // As stars get brighter they should gradually increase in size until they are very close but not get too big
        {
            if (bright > 0.01)
            {
                bright = 0.01f + ((bright - 0.01f) / 4f);
                if (bright > 0.02)
                {
                    bright = 0.02f + ((bright - 0.02f) / 8f);
                    if (bright > 0.03f)
                    {
                        bright = 0.03f + ((bright - 0.03f) / 16f);
                        if (bright > 0.04f)
                        {
                            bright = 0.04f + ((bright - 0.04f) / 4f);
                            if (bright > 0.1f) bright = 0.1f;
                        }
                    }
                }
            }
        }
        else // make faint stars fade out
        {
            float a = (bright / 0.005f);
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, a);
            if (bright > 0.005f) bright = 0.005f;
        }
        //if (printBright) print(starName + " Brightness: " + bright);
        float scale = dist * bright;
        transform.localScale = new Vector3(scale, scale, scale);
        return scale;
    }

    public void GenerateInfo(string specClass)
    {
        Random.InitState(starSeed);

        spectralClass = specClass;

        float percent = 0;
        float probFloat = Random.Range(0f, 100f);
        int ec = Physics.instance.PositionInArray(spectralClass[0]);

        for (int lum = 0; lum < Physics.instance.spectralClass[ec].luminosity.Count; lum++)
        {
            percent += Physics.instance.spectralClass[ec].luminosity[lum].percent;

            if (probFloat < percent)
            {
                luminosity = Random.Range(Physics.instance.spectralClass[ec].luminosity[lum].ValueRange.x, Physics.instance.spectralClass[ec].luminosity[lum].ValueRange.y);
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
                    numberOfPlanets = Random.Range((int)Physics.instance.spectralClass[ec].planets[planets].ValueRange.x, (int)Physics.instance.spectralClass[ec].planets[planets].ValueRange.y+1);
                    break;
                }
            }
        }
        else
            numberOfPlanets = 0;

        starName = GenerateStarName();

        for (ec = 0; ec < Physics.instance.spectralClass.Count; ec++)
        {
            if (spectralClass[0] == Physics.instance.spectralClass[ec].Type[0])
            {
                float luminRangePercent = (luminosity - Physics.instance.spectralClass[ec].luminosityRange.x) / (Physics.instance.spectralClass[ec].luminosityRange.y - Physics.instance.spectralClass[ec].luminosityRange.x); // from 0f to 1f
                int tempTemp = (int)(((Physics.instance.spectralClass[ec].TemperatureRange.y - Physics.instance.spectralClass[ec].TemperatureRange.x) * luminRangePercent) + Physics.instance.spectralClass[ec].TemperatureRange.x);
                int tempMinTemp = tempTemp - 100;
                if (tempMinTemp < Physics.instance.spectralClass[ec].TemperatureRange.x) tempMinTemp = (int)Physics.instance.spectralClass[ec].TemperatureRange.x;
                int tempMaxTemp = tempTemp + 100;
                if (tempMaxTemp > Physics.instance.spectralClass[ec].TemperatureRange.y) tempMaxTemp = (int)Physics.instance.spectralClass[ec].TemperatureRange.y;
                temperature = Random.Range(tempMinTemp, tempMaxTemp);

                spectralClass = Physics.instance.spectralClass[ec].Type + (9 - ((int)((temperature - Physics.instance.spectralClass[ec].TemperatureRange.x) /
                    ((Physics.instance.spectralClass[ec].TemperatureRange.y - Physics.instance.spectralClass[ec].TemperatureRange.x) / 10)))) + "V";

                float temperaturePercent = (temperature - Physics.instance.spectralClass[ec].TemperatureRange.x) / (Physics.instance.spectralClass[ec].TemperatureRange.y - Physics.instance.spectralClass[ec].TemperatureRange.x); // from 0f to 1f

                float combinedLumTemp = Physics.instance.spectralClass[ec].RadiusRange.x + ((Physics.instance.spectralClass[ec].RadiusRange.y - Physics.instance.spectralClass[ec].RadiusRange.x) *luminRangePercent * temperaturePercent);

                radius = combinedLumTemp;
                    //Random.Range(Physics.instance.spectralClass[ec].RadiusRange.x, Physics.instance.spectralClass[ec].RadiusRange.y);
                
                break;
            }
        }        

        radius = Mathf.Pow(5772 / (float)temperature, 2) * Mathf.Pow(luminosity / 1f, 0.5f);

    }

    private string GenerateStarName()
    {
        string[] firstNames = new string[]
        {
            "Alpha", "Beta", "Omega", "Proxima", "New", "Gamma", "Stella", "", ""
        };

        string[] middleNames = new string[]
        {
            "Centauri", "Vega", "Cyrus", "Pandora", "Celeste", "Orion", "Dawn", "Aurora", "Portia", "Andromeda", "Aquila", "Aries", "Chamaeleon", "Draco", "Gemini", "Hydra", "Hercules", "Leo", "Lynx", "Pegasus", "Phoenix", "Pisces", "Scorpius", "Ursa", "Vela", "Wolf"
        };

        string[] lastNames = new string[]
        {
            "Minor", "Major", "Nova", "Delta", "Theta", "Lambda", "", "", "", ""
        };

        string generatedStarName;
        int firstNameRandIndex = Random.Range(0, firstNames.GetLength(0));
        generatedStarName = firstNames[firstNameRandIndex];
        if (firstNames[firstNameRandIndex] != "")
            generatedStarName += " ";
        int middleNameRandIndex = Random.Range(0, middleNames.GetLength(0));
        generatedStarName += " " + middleNames[middleNameRandIndex];
        int lastNameRandIndex = Random.Range(0, lastNames.GetLength(0));
        if (lastNames[lastNameRandIndex] != "")
            generatedStarName += " " + lastNames[lastNameRandIndex];

        return generatedStarName;
    }
}