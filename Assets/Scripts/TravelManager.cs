using UnityEngine;

public class TravelManager : MonoBehaviour {
    public enum TravelMode { Galaxy, Stellar, Solar, Planetary, Proximity };
    public TravelMode travelMode = TravelMode.Stellar;
}
