using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using TMPro;

public class CombatBehaviorObjects : MonoBehaviour
{

    private float fireTimer;
    private bool canShoot;
    private bool reloading;
    public float reloadTimer;
    public float meleeRate;
    public int ammoCount;
    public float punchForce;
    public Transform centerPoint;
    public Transform firePoint;
    public Transform laserPoint;
    private PlayerBehaviorRewire PBR;
    private Player player;
    public bool canPickup;
    public bool pickup;
    public GameObject weaponPickup;
    private LineRenderer lineRenderer;
    public GameObject[] bullets;
    public GunWeapon gunRight;
    public Image gunImage;
    public TMP_Text ammoText;
    private AudioSource AS;
    public AudioClip sfx_punchHit;
    public AudioClip sfx_unload;
    public AudioClip sfx_cock;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        PBR = GetComponent<PlayerBehaviorRewire>();
        player = ReInput.players.GetPlayer(PBR.playerNumber);
        AS = GameObject.Find("Audio Source").GetComponent<AudioSource>();

        canShoot = true;
        fireTimer = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        Dead();

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, laserPoint.position);

        WeaponFunction();

        if (Mathf.Abs(player.GetAxis("AimVertical")) > 0 || Mathf.Abs(player.GetAxis("AimHorizontal")) > 0)
        {
            centerPoint.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(player.GetAxis("AimVertical"), player.GetAxis("AimHorizontal")) * Mathf.Rad2Deg));
        }

        DropWeapon();

        if (canPickup)
        {
            PickupWeapon();
        }

    }

    void Shoot()
    {
        if (canShoot && ammoCount > 0 && !reloading && !PBR.dodging)
        {
            //bullets[ammoCount - 1].SetActive(false);         
            ammoCount -= 1;
            ammoText.text = ammoCount + "/" + gunRight.maxAmmo;
            gunRight.Action(firePoint);
        }
    }

    void Reload()
    {
        reloadTimer += Time.deltaTime;

        ammoText.text = "Reloading";

        if (reloadTimer >= gunRight.reloadTime)
        {
            ammoCount = gunRight.maxAmmo;
            ammoText.text = ammoCount + "/" + gunRight.maxAmmo;

            if(AS != null)
            {
                AS.pitch = Random.Range(0.8f, 1.1f);
                AS.PlayOneShot(sfx_cock);
            }

            reloading = false;
        }
    }

    void WeaponFunction()
    {
        if(gunRight != null)
        {
            if (fireTimer < gunRight.fireRate)
            {
                fireTimer += Time.deltaTime;
            }

            if(gunRight.weaponType == GunWeapon.weaponTypes.Semi)
            {
                if (player.GetButtonDown("Shoot"))
                {
                    if (fireTimer >= gunRight.fireRate)
                    {
                        fireTimer = 0f;
                        Shoot();
                    }

                }
            }

            if (gunRight.weaponType == GunWeapon.weaponTypes.Auto)
            {
                if (player.GetButton("Shoot"))
                {
                    if (fireTimer >= gunRight.fireRate)
                    {
                        fireTimer = 0f;
                        Shoot();
                    }
                }
            }

            if (player.GetButtonDown("Reload") && !reloading && ammoCount != gunRight.maxAmmo)
            {
                reloadTimer = 0f;
                reloading = true;

                if(AS != null)
                {
                    AS.pitch = Random.Range(0.8f, 1.1f);
                    AS.PlayOneShot(sfx_unload);
                }
            }
            if (player.GetButtonDown("Shoot") && !reloading && ammoCount <= 0)
            {
                reloadTimer = 0f;
                reloading = true;
                if (AS != null)
                {
                    AS.pitch = Random.Range(0.9f, 1.1f);
                    AS.PlayOneShot(sfx_unload);
                }
            }
            if (reloading)
            {
                Reload();
            }
        } else
        {
            if (fireTimer < meleeRate)
            {
                fireTimer += Time.deltaTime;
            }

            if (player.GetButtonDown("Shoot"))
            {
                if (fireTimer >= meleeRate)
                {
                    Punch();
                }
            }
        }

    }

    void Punch()
    {
        Debug.Log("punch");
        PBR.animator.SetTrigger("Punch");

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.GetComponent<Rigidbody2D>() != null && hitCollider.gameObject != this.gameObject && hitCollider.GetComponent<StatusBehavior>())
            {
                if(hitCollider.GetComponent<ExplosiveBarrel>() != null)
                {
                    
                    hitCollider.GetComponent<ExplosiveBarrel>().playerPunched = this.gameObject;
                    hitCollider.GetComponent<ExplosiveBarrel>().punched = true;

                }

                hitCollider.GetComponent<StatusBehavior>().KnockForce(punchForce, centerPoint.transform.right);               

                if (hitCollider.gameObject.tag == "Player")
                {
                    hitCollider.GetComponent<StatusBehavior>().Stunned();
                } else
                {
                    hitCollider.GetComponent<Rigidbody2D>().AddTorque(20f);
                }

                if (AS != null)
                {
                    AS.pitch = Random.Range(0.8f, 1.2f);
                    AS.PlayOneShot(sfx_punchHit);
                }
            }
        }
        fireTimer = 0f;
    }

    void PickupWeapon()
    {
        if (player.GetButtonDown("Drop") && gunRight == null && !weaponPickup.GetComponent<WeaponPickup>().startCDTimer)
        {
            weaponPickup.GetComponent<WeaponPickup>().OnPickup();
            gunRight = weaponPickup.GetComponent<WeaponPickup>().weapon;
            gunImage.sprite = gunRight.artwork;
            ammoCount = gunRight.maxAmmo;
            ammoText.text = ammoCount + "/" + gunRight.maxAmmo;
            canPickup = false;

        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WeaponPickup"))
        {
            weaponPickup = collision.gameObject;
            if (!collision.gameObject.GetComponent<WeaponPickup>().startCDTimer)
            {
                canPickup = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WeaponPickup"))
        {
            weaponPickup = null;
            canPickup = false;
        }
    }

    void DropWeapon()
    {
        if (player.GetButtonDown("Drop") && gunRight != null)
        {
            ammoText.text = "0/0";
            ammoCount = 0;
            gunRight = null;
            gunImage.sprite = null;
        }
    }

    public void Dead()
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            this.enabled = false;
        }
    }
}
