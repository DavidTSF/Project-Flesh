using UnityEngine;
using TMPro;

public class FloatingMessage : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float duration = 1.5f;
    public float fadeDuration = 0.5f;
    public Vector2 rotationRange = new Vector2(-10f, 10f);

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI text;
    private float timer;
    

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponent<TextMeshProUGUI>();
        transform.Rotate(0f, 0f, Random.Range(rotationRange.x, rotationRange.y));
    }

    public void SetText(string message)
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        text.text = message;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (timer > duration - fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - (duration - fadeDuration)) / fadeDuration);
        }

        if (timer > duration)
        {
            Destroy(gameObject);
        }
    }
}