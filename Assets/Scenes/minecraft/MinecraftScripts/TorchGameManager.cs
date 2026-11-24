using System.Collections;
using UnityEngine;

public class TorchGameManager : MonoBehaviour
{
    [SerializeField] private TorchController[] torches; //array of all torches in the scene

    [Header("Loose/Jumpscare UI")]
    [SerializeField] private GameObject jumpscareCanvas; // parent canvas in front of camera
    [SerializeField] private float jumpscareDuration = 2.0f;
    [SerializeField] private jumpscareZoom jumpscare;//serialize private, only this script can access it but shows in inspecter anyways:3

    [Header("Win Clock")]
    [SerializeField] private Transform clockTransform; //daynight quad object
    [SerializeField] private float nightToMorningTime = 60f; //60 sekunder overlev(ændret)
    [SerializeField] private float startNightAngle = 180f; //rotation at night start
    [SerializeField] private float endMorningAngle = 0f; //rotation at morning end

    [Header("Win UI")]
    [SerializeField] private GameObject winCanvas; //  win canvas

    [Header("Intro UI")]
    [SerializeField] private GameObject introCanvas; // intro canvas
    [SerializeField] private float introDuration = 2f;

    [Header("Door Audio")]
    [SerializeField] private AudioSource doorSource;
    [SerializeField] private AudioSource zombieSource;

    [SerializeField] private AudioClip[] doorClips;   // Door, Door1
    [SerializeField] private AudioClip[] zombieClips; // Zombie, Zombie1, Zombie2

    [SerializeField, Range(0f, 1f)]
    private float doorStartPercent = 0.6f;

    private bool doorPhaseStarted = false;


    private bool gameOverTriggered = false;
    private bool winTriggered = false;
    private float timer = 0f;

    private void Start()
    {

        foreach (var torch in torches)
        {
            torch.OnTorchExtinguished += CheckGameOver; //event driven, notiffy gamemanager when torch goes out
        }

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);

        if (winCanvas != null)
            winCanvas.SetActive(false);

        if ( introCanvas != null)
        {
            introCanvas.SetActive(true);
            StartCoroutine(HideIntroAfterSeconds());
        }

        // force clock to start at night
        if (clockTransform != null)
        {
            Vector3 e = clockTransform.localEulerAngles;
            e.z = startNightAngle; //180 degrees at start z
            clockTransform.localEulerAngles = e;
        }
    }

    private void Update()
    {
        if (gameOverTriggered || winTriggered) return; //stops gameplay when either triggered

        // progress timer
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / nightToMorningTime); //de 60 sekunder

        if (!doorPhaseStarted && t >= doorStartPercent) 
        {
            doorPhaseStarted = true;
            StartCoroutine(DoorAndZombieSequence());
        }

        // rotate clock from 180 -> 0
        if (clockTransform != null)
        {
            float z = Mathf.Lerp(startNightAngle, endMorningAngle, t);
            Vector3 e = clockTransform.localEulerAngles;
            e.z = z;
            clockTransform.localEulerAngles = e;
        }

        // win check
        if (t >= 1f) //time is up
        {
            winTriggered = true;
            StartCoroutine(WinSequence());
        }
    }

    private void CheckGameOver()
    {
        if (gameOverTriggered) return;
        StartCoroutine(CheckGameOverNextFrame());
    }

    private IEnumerator CheckGameOverNextFrame()
    {
        Debug.Log("All torches are out!");

        yield return null; // wait one frame so all torches update IsLit make sure correct

        foreach (var torch in torches)
        {
            if (torch.IsLit) //are there any lit torches left break this
                yield break;
        }

        gameOverTriggered = true;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Debug.Log("GAME OVER SEQUENCE STARTED");

        StopAllLoopingSounds();

        AudioManager.instance.Play("Death");

        // show jumpscare
        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(true);

        // start the zoom animation
        if (jumpscare != null)
            jumpscare.Trigger();


        yield return new WaitForSeconds(jumpscareDuration);

        // stop time (freezes physics/gameplay)
        Time.timeScale = 0f;

        // can also hide the jumpscare after freezing:
        // jumpscareCanvas.SetActive(false);
    }

    private IEnumerator WinSequence()
    {
        Debug.Log("YOU SURVIVED UNTIL MORNING! WIN!");

        StopAllLoopingSounds();

        AudioManager.instance.Play("WonXP");

        if (winCanvas != null)
            winCanvas.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0f;
    }

    private IEnumerator HideIntroAfterSeconds()
    {
        yield return new WaitForSeconds(introDuration);

        if (introCanvas != null)
        {
            CanvasFade fade = introCanvas.GetComponent<CanvasFade>();
            fade.FadeOut(1f); // fade duration (1 second is perfect)
        }
    }

    private IEnumerator DoorAndZombieSequence()
    {
        // Start door banging loop
        StartCoroutine(DoorBangingLoop());

        // Wait 3–5 sec, then start zombie loop
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(1f, 2f));
        StartCoroutine(ZombieNoiseLoop());
    }

    private IEnumerator DoorBangingLoop()
    {
        while (!gameOverTriggered && !winTriggered)
        {
            if (doorSource != null && doorClips.Length > 0)
            {
                // pick and play a random bang
                var clip = doorClips[UnityEngine.Random.Range(0, doorClips.Length)];
                doorSource.clip = clip;
                doorSource.Play();

                // wait EXACTLY until the clip is done
                yield return new WaitForSecondsRealtime(clip.length);
                //wait a random short time before next bang
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.2f, 1.0f));
            }
            else
            {
                yield return null; // safety if missing refs
            }
        }
    }

    private IEnumerator ZombieNoiseLoop()
    {
        while (!gameOverTriggered && !winTriggered)
        {
            if (zombieSource != null && zombieClips.Length > 0)
            {
                var clip = zombieClips[UnityEngine.Random.Range(0, zombieClips.Length)];
                zombieSource.clip = clip;
                zombieSource.Play();

                yield return new WaitForSecondsRealtime(clip.length);
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(2f, 3f));
            }
            else
            {
                yield return null;
            }
        }
    }

    private void StopAllLoopingSounds()
    {
        if (doorSource != null)
            doorSource.Stop();

        if (zombieSource != null)
            zombieSource.Stop();
    }
}
