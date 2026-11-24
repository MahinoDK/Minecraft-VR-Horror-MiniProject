using UnityEngine;

public class jumpscareZoom : MonoBehaviour //placed on zombieface under camera cánvas
{
    public float zoomSpeed = 7f;
    public float maxScale = 2.8f;//how big the jumpscare gets until time zoom stops

    private bool active = false;

    public void Trigger() //called in gamemanger zombieface dragged in slot on gamemanager object
    {
       active = true;
        transform.localScale = Vector3.one * 0.1f; //start tiny
    }

    private void Update()
    {
       if (!active) return;

        AudioManager.instance.Play("Death");

        transform.localScale = Vector3.Lerp(
           transform.localScale,
           Vector3.one * maxScale,
           Time.unscaledDeltaTime * zoomSpeed


       );

        if (transform.localScale.x >= maxScale * 0.98f)
         {
              active = false;
            Time.timeScale = 0f; //freeze time
        }
    }
}
