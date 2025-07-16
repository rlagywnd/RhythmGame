using UnityEngine;

public class Star : MonoBehaviour
{
    private Vector3 baseScale;
    private float bounceTimer;
    private bool isBouncing = false;

    [SerializeField] private float bounceDuration = 0.18f;
    [SerializeField] private float bounceScale = 1.2f; // ºñÀ²
    [SerializeField] private AnimationCurve bounceCurve;

    void Awake()
    {
        baseScale = transform.localScale;
        if (bounceCurve == null || bounceCurve.length == 0)
            bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 


    }

    void Update()
    {
        if (!isBouncing) return;

        bounceTimer += Time.deltaTime;
        float t = bounceTimer / bounceDuration;

        if (t >= 1f)
        {
            isBouncing = false;
            transform.localScale = baseScale; 
            return;
        }

        float scale = Mathf.Lerp(bounceScale, 1f, bounceCurve.Evaluate(t));
        transform.localScale = baseScale * scale;
    }
    public void Stop()
    {
        isBouncing = false;
        transform.localScale = baseScale;
    }
    public void TriggerBounce()
    {
        bounceTimer = 0f;
        isBouncing = true;
    }
}
