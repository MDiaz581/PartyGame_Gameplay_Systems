using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class ExplosionBehavior : MonoBehaviour
{
    [HideInInspector]
    public float explosionMultiplier;

    [HideInInspector]
    public float explosionLiftMultiplier;

    [HideInInspector]
    public float explosionDistance;

    [HideInInspector]
    public float knockbackDistance;

    [HideInInspector]
    public float kbExplosionMulitplierModifier;

    public GameObject[] ps;

    private AudioSource AS;

    public AudioClip sfx_explosion;

    private MMFeedbacks _screenShake;



    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    public void Explode()
    {
        foreach (var subPS in ps)
        {
            var explosion = Instantiate(subPS, transform.position, transform.rotation);
            explosion.AddComponent<CleanupScript>();
        }

        _screenShake = GameObject.Find("Feedbacks/ExplosionShake").GetComponent<MMFeedbacks>();

        _screenShake?.PlayFeedbacks();

        AS = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        if (AS != null)
        {
            AS.PlayOneShot(sfx_explosion);
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Player" && !hitCollider.gameObject.GetComponent<PlayerBehaviorRewire>().isDead)
            {
                var dir = hitCollider.transform.position - transform.position;

                if (hitCollider.GetComponent<Rigidbody2D>() != null)
                {
                    hitCollider.GetComponent<Rigidbody2D>().AddForce(dir * explosionMultiplier, ForceMode2D.Impulse);
                    hitCollider.GetComponent<Rigidbody2D>().AddForce(Vector2.up * explosionLiftMultiplier, ForceMode2D.Impulse);
                }

                hitCollider.gameObject.GetComponent<PlayerBehaviorRewire>().Die();
            }

            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("InteractableObject"))
            {
                if (hitCollider.gameObject.GetComponent<ExplosiveBarrel>() != null)
                {

                    hitCollider.gameObject.GetComponent<ExplosiveBarrel>().Explode();

                }

            }
        }

        Collider2D[] knockbackColliders = Physics2D.OverlapCircleAll(transform.position, explosionDistance + knockbackDistance);

        foreach (var knockbackCollider in knockbackColliders)
        {

            if (knockbackCollider.gameObject.tag == "Player")
            {
                var dir = knockbackCollider.transform.position - transform.position;

                if (knockbackCollider.GetComponent<Rigidbody2D>() != null && Vector2.Distance(knockbackCollider.transform.position, transform.position) > explosionDistance)
                {
                    knockbackCollider.GetComponent<Rigidbody2D>().AddForce(dir * explosionMultiplier / Vector2.Distance(knockbackCollider.transform.position, transform.position) / kbExplosionMulitplierModifier, ForceMode2D.Impulse);
                    knockbackCollider.GetComponent<Rigidbody2D>().AddForce(Vector2.up * explosionLiftMultiplier / 2, ForceMode2D.Impulse);
                }
            }
        }

        

        this.enabled = false;

        Destroy(this);
    }

}
