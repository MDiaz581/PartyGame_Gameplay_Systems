using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class GunWeapon : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

    public GameObject model;

    public int maxAmmo;

    public float fireRate;

    public float reloadTime;

    public float accuracy;

    public GameObject projectile;

    public float projectileSpeed;

    public enum weaponTypes { Semi, Auto, Bolt };

    public weaponTypes weaponType;


    public void Action(Transform firePoint)
    {

        projectile.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile.GetComponent<ProjectileScript>().projectileAccuracy = Random.Range(-accuracy, accuracy);
        Instantiate(projectile, firePoint.position, firePoint.rotation);

    }


}
