using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TorchRelightTrigger : MonoBehaviour
{
    [SerializeField] private TorchController torch; //drag torch parent in here this is collider

    private void Awake()
    {
        if (!torch) torch = GetComponentInParent<TorchController>(); //fallback to find it if i dont drag it in
        GetComponent<Collider>().isTrigger = true;//to detect overlap and interaction
    }

    private void OnTriggerStay(Collider other) //called every frame another collider is inside trigger
    {
        var flint = other.GetComponentInParent<FlintAndSteelController>(); //does this object have flint and steel controller script on it or parent
        if (flint == null) return;

        // only relight while flint is actually sparking
        torch.TryLightFromFlint(flint.IsSparking);   //isSparking is public bool in flintandsteelcontroller and trylightfromflint is public method in torchcontroller to refill torch
    }
}