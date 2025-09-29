using UnityEngine;

public class JunkGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public GameObject[] bullets;
    [SerializeField] public float fireRate;
    [SerializeField] public int magMax;
    public int inMag;
    public AudioSource gunSound;
    public AudioClip soundEffect;
    public Transform shootPos;
    private int lastindex = -1;



    void Start()
    {
        inMag = magMax;
    }

    // Update is called once per frame
  public GameObject randomBullet()
    {
        int randombullet; 

        do
        {
            randombullet = Random.Range(0, bullets.Length);
        } while (randombullet == lastindex);

        lastindex = randombullet;

        return bullets[randombullet];
    }
}
