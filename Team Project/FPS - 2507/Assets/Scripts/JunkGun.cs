using UnityEngine;

public class JunkGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public damage[] bullets;
    [SerializeField] public float fireRate;
    [SerializeField] public int magMax;
    public int inMag;
    public AudioSource gunSound;
    public AudioClip soundEffect;
    public Transform shootPos;



    void Start()
    {
        inMag = magMax;
    }

    // Update is called once per frame
  public damage randomBullet()
    {
        int randombullet = Random.Range(0, bullets.Length);

        return bullets[randombullet];
    }
}
