using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private DamageText damageTextPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Spawn(int amount, Vector3 worldPos)
    {
        if (damageTextPrefab == null) return;
        var dt = Instantiate(damageTextPrefab, worldPos, Quaternion.identity);
        dt.Initialize(amount, worldPos);
    }
}
