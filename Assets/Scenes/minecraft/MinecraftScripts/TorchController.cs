using UnityEngine;

public class TorchController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LightFlicker flicker; 
    [SerializeField] private ParticleSystem[] flameSystems;
    [SerializeField] private AudioSource fireAudio;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime = 60f; //duration in seconds
    [SerializeField] private float startOffset = 0f; //how far down the bar this torch starts

    private float currentLifetime;
    private bool isLit;

    private float[] initialSizes;

    //gamemanger here
    public System.Action OnTorchExtinguished; //event to notify when torch goes out to torchgamemanager
    public bool IsLit => isLit; //other scripts can read this value


    private void Awake()
    {
        if (!fireAudio)
            fireAudio = GetComponent<AudioSource>(); //i can drag in inspector but it can find it too

        if (!flicker) flicker = GetComponentInChildren<LightFlicker>();

        // Cache starting sizes for all particle systems
        initialSizes = new float[flameSystems.Length]; //store size so shink and come back to normal
        for (int i = 0; i < flameSystems.Length; i++)
        {
            var main = flameSystems[i].main;
            initialSizes[i] = main.startSizeMultiplier;
        }

        // Apply starting offset so some torches are already partly “used up”
        currentLifetime = Mathf.Clamp(maxLifetime - startOffset, 0f, maxLifetime);

        if (currentLifetime <= 0f)
            Extinguish();
        else
            Ignite();
    }

    private void Update()
    {
        if (!isLit) return; //if lit fade over time

        currentLifetime -= Time.deltaTime;
        float t = Mathf.Clamp01(currentLifetime / maxLifetime);   // 1 -> 0 over time

        // Fade light
        if (flicker != null)
            flicker.brightnessFactor = t;  // 1 = full bright, 0 = off

        // Fade ALL flame particle sizes
        for (int i = 0; i < flameSystems.Length; i++)
        {
            var main = flameSystems[i].main;
            main.startSizeMultiplier = Mathf.Lerp(0.05f, initialSizes[i], t); //t start size 1 to 0 nothing
        }

        if (currentLifetime <= 0f)
            Extinguish();
    }

    public void Refill()
    {
        currentLifetime = maxLifetime; //if refill happens full lifetime + ignite it
        Ignite();
    }

    private void Ignite()
    {
        isLit = true;

        if (fireAudio != null && !fireAudio.isPlaying)
            fireAudio.Play();

        if (flicker != null)
            flicker.brightnessFactor = 1f; //full brightness

        foreach (var ps in flameSystems) //particle systems play
        {
            if (!ps.isPlaying)
                ps.Play();
        }
    }

    private void Extinguish()
    {
        // avoid firing event multiple times
        if (!isLit && currentLifetime == 0f)
            return;

        if (fireAudio != null && fireAudio.isPlaying)
            fireAudio.Stop();

        AudioManager.instance.PlayRandomCave(
    "Cave1", "Cave2", "Cave3", "Cave4", "Cave5",
    "Cave6", "Cave7", "Cave8", "Cave9", "Cave10", "Cave11");

        // fire the event ONCE when we transition from lit ? out
        if (isLit)
            OnTorchExtinguished?.Invoke(); //only trigger when it goes out from lit to out - gamemanger checks if that was last torch

        isLit = false;
        currentLifetime = 0f;

        if (flicker != null)
            flicker.brightnessFactor = 0f; //turn brightness off

        foreach (var ps in flameSystems) //stop particle systems
        {
            if (ps.isPlaying)
                ps.Stop();
        }
    }

    public void TryLightFromFlint(bool flintIsSparking) //To call with collisison check
    {
        if (!flintIsSparking) return; //se torchRelightTrigger script
        Refill();
    }
}

