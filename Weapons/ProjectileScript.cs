using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private Rigidbody2D RB;

    public float speed;

    public float projectileAccuracy;

    private float destroyTimer;

    private AudioSource AS;

    public AudioClip sfx_shot;

    public AudioClip sfx_hit;

    public GameObject explosion;


    public enum weaponTypes { Standard, Explosive };

    public weaponTypes weaponType;

    [Header("Explosives Modifier")]

    [Tooltip("This is the size of the killing radius of the explosion")]
    public float explosionDistance;

    [Tooltip("This is the force the explosion applies in the direction away from the center of the explosion")]
    public float explosionMultiplier;

    [Tooltip("This is an added upwards force to the explosion so it knocks you off your feet in a sense")]
    public float explosionLiftMultiplier;

    [Tooltip("This is the ADDITIVE size of the killing radius of the explosion, this does not kill players, but knocks them away from the explosion")]
    public float knockbackDistance;

    [Tooltip("This is modifies the force the explosion when in knockback range, making it stronger or weaker. Higher values = weaker knockback")]
    public float kbExplosionMulitplierModifier;

    private void Awake()
    {
        //Give our values from the rocket prefab to the explosion prefab
        explosion.GetComponent<ExplosionBehavior>().explosionDistance = explosionDistance;
        explosion.GetComponent<ExplosionBehavior>().explosionMultiplier = explosionMultiplier;
        explosion.GetComponent<ExplosionBehavior>().explosionLiftMultiplier = explosionLiftMultiplier;
        explosion.GetComponent<ExplosionBehavior>().knockbackDistance = knockbackDistance;
        explosion.GetComponent<ExplosionBehavior>().kbExplosionMulitplierModifier = kbExplosionMulitplierModifier;

        RB = GetComponent<Rigidbody2D>();

        AS = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (weaponType == weaponTypes.Standard)
        {
            RB.AddForce(transform.right * speed, ForceMode2D.Impulse);
            RB.AddForce(transform.up * projectileAccuracy, ForceMode2D.Impulse);

        }

        

        if (AS != null)
        {
            AS.pitch = Random.Range(0.8f, 1.2f);
            AS.PlayOneShot(sfx_shot);
        }

    }
    private void FixedUpdate()
    {
        if (weaponType == weaponTypes.Explosive)
        {

            RB.AddForce(transform.right * speed, ForceMode2D.Force);
            RB.AddForce(transform.up * projectileAccuracy, ForceMode2D.Force);

        }
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer += Time.deltaTime;
        if(destroyTimer > 10f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (weaponType == weaponTypes.Standard)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (AS != null)
                {
                    AS.pitch = Random.Range(0.8f, 1.2f);
                    AS.PlayOneShot(sfx_hit);
                }

                //collision.gameObject.layer = 10;

                collision.gameObject.GetComponent<PlayerBehaviorRewire>().Die();

            }
        } 
        else if (weaponType == weaponTypes.Explosive)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }


        Destroy(this.gameObject);
    }
}
