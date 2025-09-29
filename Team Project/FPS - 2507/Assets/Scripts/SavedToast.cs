using UnityEngine;

public class SavedToast : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private float holdTime = 1.0f;
    [SerializeField] private float fadeTime = 0.35f;

    void Reset()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(CoShow());
    }

    private System.Collections.IEnumerator CoShow()
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSecondsRealtime(holdTime);

        t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }
        group.alpha = 0f;
    }
}
