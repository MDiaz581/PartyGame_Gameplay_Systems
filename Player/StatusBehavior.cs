using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBehavior : MonoBehaviour
{

    public float stunTimer;


    [Tooltip("This is the base speed of which the object will get knockedback by")]
    public float innateKnockback;

    [Tooltip("This is an additive of the knockback speed which the object will get knockedback by while in the air")]
    public float inAirKnockback;

    private float time;

    private bool stunned;

    public bool inAir;

    [SerializeField]
    private LayerMask hitMask;

    [SerializeField]
    private Vector3 offset;

    private PlayerBehaviorRewire PBR;

    private CombatBehaviorObjects CBO;

    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        PBR = GetComponent<PlayerBehaviorRewire>();

        CBO = GetComponent<CombatBehaviorObjects>();

        if(PBR != null)
        {
            animator = PBR.animator;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(hitMask != LayerMask.NameToLayer("Nothing"))
        {
            groundChecker();
        }


        if (stunned)
        {
            time += Time.deltaTime;
        

            if(time >= stunTimer)
            {
                CBO.enabled = true;
                PBR.enabled = true;

                animator.SetTrigger("Return");

                time = 0f;

                stunned = false;

                Debug.Log("Player " + PBR.playerNumber + " is no longer stunned");
            }
        }
        
    }

    public void Stunned()
    {
        if(PBR != null && CBO != null && !PBR.isDead)
        {
            Debug.Log("Player " + PBR.playerNumber + 1 + " is stunned");
            stunned = true;


            animator.SetTrigger("Stunned");

            CBO.enabled = false;
            PBR.enabled = false;
        }

    }

    public void KnockForce(float force, Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);

        if (inAir)
        {
            GetComponent<Rigidbody2D>().AddForce(direction * force * (innateKnockback + inAirKnockback), ForceMode2D.Impulse);
        } else
        {
            GetComponent<Rigidbody2D>().AddForce(direction * force * innateKnockback, ForceMode2D.Impulse);
        }


    }

    private void groundChecker()
    {
        RaycastHit2D groundChecker = Physics2D.CircleCast(transform.position + offset, 0.1f, -Vector2.up, 100f, hitMask);

        if (groundChecker.collider != null)
        {
            if (Mathf.Abs(groundChecker.point.y - transform.position.y) < 0.6f)
            {
                inAir = false;
            }
            else
            {
                inAir = true;
            }
        }
    }

}
