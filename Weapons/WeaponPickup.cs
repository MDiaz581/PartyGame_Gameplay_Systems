using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    public GunWeapon weapon;

    private GameObject model;

    public bool startCDTimer;

    public float respawnTime;


    // Start is called before the first frame update
    void Start()
    {
        model = weapon.model;

        Instantiate(model, transform.position, model.transform.rotation, transform);

        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnPickup()
    {

        this.gameObject.GetComponent<MeshRenderer>().enabled = true;

        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(Cooldown(respawnTime));

        startCDTimer = true;

    }

    private IEnumerator Cooldown(float CDTime)
    {
       
        yield return new WaitForSeconds(CDTime);

        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;

        startCDTimer = false;

    }
}
