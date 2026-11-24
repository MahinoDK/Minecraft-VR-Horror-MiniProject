using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CanvasFade : MonoBehaviour
{
    private CanvasGroup group;//to control group fade - fade ui, disable interaction, disable raycasts

    private void Awake()
    {
        group = GetComponent<CanvasGroup>(); //find existing canvas group
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void FadeOut(float duration) //public function i call from other scripts
    {
        StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration) //where the fade happens
    {
        float t = 0f; //time into fade
        float startAlpha = group.alpha;//start transparency

        while (t < duration) //runs untill fade is complete
        {
            t += Time.unscaledDeltaTime; //It still fades even if timeScale = 0 even if game is paused
            group.alpha = Mathf.Lerp(startAlpha, 0f, t / duration); //This smoothly transitions from: startAlpha → 0 fade
            yield return null;
        }

        group.alpha = 0f; //make fully transparent
        gameObject.SetActive(false); //disable the whole canvas object
    }
}