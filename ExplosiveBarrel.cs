using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

public class ExplosiveBarrel : MonoBehaviour
{

    [Tooltip("This is the force the explosion applies in the direction away from the center of the explosion")]
    public float explosionMultiplier;

    [Tooltip("This is an added upwards force to the explosion so it knocks you off your feet in a sense")]
    public float explosionLiftMultiplier;

    [Tooltip("This is the size of the killing radius of the explosion")]
    public float explosionDistance;

    [Tooltip("This is the ADDITIVE size of the killing radius of the explosion, this does not kill players, but knocks them away from the explosion")]
    public float knockbackDistance;

    public GameObject md_exploded;
    public GameObject md_whole;

    public GameObject[] ps;

    private AudioSource AS;

    public AudioClip sfx_explosion;

    public GameObject playerPunched;

    private Transform ExplosionHitbox;

    public bool punched;

    public float invincibilityTime;

    private MMFeedbacks _screenShake;

    // Start is called before the first frame update
    void Start()
    {
        AS = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        ExplosionHitbox = transform.Find("ExplosionHitbox");

        _screenShake = GameObject.Find("Feedbacks/ExplosionShake").GetComponent<MMFeedbacks>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 100f)
        {
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(ExplosionHitbox.position, ExplosionHitbox.localScale, 0f);

            foreach (var hitCollider in hitColliders)
            {
                Debug.Log(hitCollider);
                if (hitCollider.gameObject.tag == "Player" && hitCollider.gameObject != playerPunched)
                {
                    Explode();
                }
            }
        }
        if (punched)
        {
            StartCoroutine(IFrames(invincibilityTime));
        }
    }


    public void Explode()
    {
        if (AS != null)
        {
            AS.PlayOneShot(sfx_explosion);            
        }

        _screenShake?.PlayFeedbacks();

        md_whole.SetActive(false);
        gameObject.layer = 10;
        md_exploded.SetActive(true);

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionDistance);

        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.gameObject.tag == "Player")
            {
                var dir = hitCollider.transform.position - transform.position;

                if (hitCollider.GetComponent<Rigidbody2D>() != null)
                {
                    hitCollider.GetComponent<Rigidbody2D>().AddForce(dir * explosionMultiplier, ForceMode2D.Impulse);
                    hitCollider.GetComponent<Rigidbody2D>().AddForce(Vector2.up * explosionLiftMultiplier, ForceMode2D.Impulse);
                }

                //hitCollider.gameObject.layer = LayerMask.NameToLayer("Dead");
                hitCollider.gameObject.GetComponent<PlayerBehaviorRewire>().Die();
            }
        }

        Collider2D[] knockbackColliders = Physics2D.OverlapCircleAll(transform.position, explosionDistance + 1f);

        foreach (var knockbackCollider in knockbackColliders)
        {

            if (knockbackCollider.gameObject.tag == "Player")
            {
                var dir = knockbackCollider.transform.position - transform.position;

                if (knockbackCollider.GetComponent<Rigidbody2D>() != null && Vector2.Distance(knockbackCollider.transform.position, transform.position) > explosionDistance)
                {
                    knockbackCollider.GetComponent<Rigidbody2D>().AddForce(dir * explosionMultiplier / Vector2.Distance(knockbackCollider.transform.position, transform.position), ForceMode2D.Impulse);
                    knockbackCollider.GetComponent<Rigidbody2D>().AddForce(Vector2.up * explosionLiftMultiplier / 2, ForceMode2D.Impulse);
                }
            }
        }

        foreach(var subPS in ps)
        {
            var explosion = Instantiate(subPS, transform.position, transform.rotation);
            explosion.AddComponent<CleanupScript>();
        }
        

        this.enabled = false;

        Destroy(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    
        if(collision.gameObject.layer == 11 && !punched)
        {
            Explode();
        }

    }

    private IEnumerator IFrames(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        punched = false;
    }
}
