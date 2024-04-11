using System.Collections;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float lifetime = 0.25f;
    public float inflationDuration = 0.1f;
    public float maxScale = 2.0f;

    void Start()
    {
        StartCoroutine(InflateAndDestroy());
    }

    IEnumerator InflateAndDestroy()
    {
        // Scale up effect
        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < inflationDuration)
        {
            float scale = Mathf.Lerp(originalScale.x, maxScale, elapsedTime / inflationDuration);
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(maxScale, maxScale, maxScale);
        // Wait for a short duration before destroying
        yield return new WaitForSeconds(lifetime - inflationDuration);
        Destroy(gameObject);
    }
}
