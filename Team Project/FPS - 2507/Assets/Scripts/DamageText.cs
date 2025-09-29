using UnityEngine;
using TMPro;
using System.Data;

public class DamageText : MonoBehaviour
{
    [Header("Motion")]
    public float lifetime = 3f;
    public float riseSpeed = 1.5f;
    public float horizontalJitter = 0.2f;

    [Header("Visuals")]
    public AnimationCurve alphaOverLife = AnimationCurve.EaseInOut(0, 1, 1, 0);
    public AnimationCurve scaleOverLife = AnimationCurve.EaseInOut(0, 0.9f, 1, 1.2f);

    private float _age;
    private TMP_Text _text;
    private Transform _camera;
    private Color _baseColor;
    private Vector3 _spawnPos;

    public void Initialize(int amount, Vector3 worldPos)
    {
        if (_text == null) _text = GetComponent<TMP_Text>();
        if (_camera == null && Camera.main != null) _camera = Camera.main.transform;

        _spawnPos = worldPos;

        Vector2 rand = Random.insideUnitCircle * horizontalJitter;
        _spawnPos += new Vector3(rand.x, 0f, rand.y);

        _text.text = amount.ToString();
        _baseColor = _text.color;
        _age = 0f;

        transform.position = _spawnPos;
    }

    void Update()
    {
        if (_camera == null && Camera.main != null) _camera = Camera.main.transform;

        _age += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(_age / lifetime);

        Vector3 pos = _spawnPos + Vector3.up * (riseSpeed * _age);
        transform.position = pos;

        if (_camera != null)
        {
            transform.forward = _camera.forward;
        }

        float a = alphaOverLife.Evaluate(t);
        float s = scaleOverLife.Evaluate(t);
        _text.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, a);
        transform.localScale = Vector3.one * s;

        if (_age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
