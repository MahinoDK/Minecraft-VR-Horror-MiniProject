using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlintAndSteelController : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparkParticles;

    public bool IsSparking { get; private set; } //Encapsulation other scripts can read - use to call IsSparking in torchrelighttrigger (only this script can change it

    public void OnActivate(ActivateEventArgs _) //we dont manually call here, XR interaction calls it when we pull the trigger
    {
        IsSparking = true; //so other scripts can read this like torches
        if (sparkParticles != null && !sparkParticles.isPlaying)
            sparkParticles.Play();

        AudioManager.instance.PlayAtPosition("Spark", transform.position); 
    }

    public void OnDeactivate(DeactivateEventArgs _)
    {
        IsSparking = false;
        if (sparkParticles != null && sparkParticles.isPlaying)
            sparkParticles.Stop();
    }
}
