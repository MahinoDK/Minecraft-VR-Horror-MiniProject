using UnityEngine;

[RequireComponent(typeof(Light))] //this gamobject must have a light component - defensive

public class LightFlicker : MonoBehaviour
{

    private Light lightToFlicker; //light component to flicker
    [SerializeField, Range(0f, 3f)] private float minIntensity = 0.5f; //range adds slider in inspector
    [SerializeField, Range(0f, 3f)] private float maxIntensity = 1.2f; //The light will choose a random value between these
    [SerializeField, Min(0f)] private float timeBetweenIntensity = 0.1f; //how often to change intensity
    private float currentTimer; //measure time between changes

    public float brightnessFactor = 1f;  // 1 = full strength, 0 = off

    private void Awake()
    {
        if (lightToFlicker == null)
        {
            lightToFlicker = GetComponent<Light>();//finds light component on this game object
        }

        ValidateIntensityBounds(); //play
    }

    private void Update()
    {
        currentTimer+= Time.deltaTime;
        if (!(currentTimer >= timeBetweenIntensity)) return; //if not enough time has passed, exit
        float baseIntensity = Random.Range(minIntensity, maxIntensity);
        lightToFlicker.intensity = baseIntensity * brightnessFactor;   // fade here -random flicker and lifetimefade

        currentTimer = 0;
    }

    private void ValidateIntensityBounds() //ensures min is not greater than max
    {
        if (!(minIntensity > maxIntensity))
        {
            return;
        }
        Debug.LogWarning($"Min intensity {minIntensity} is greater than max intensity {maxIntensity}. Swapping values.");
        (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
    }
}
