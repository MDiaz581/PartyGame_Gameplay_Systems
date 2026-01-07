using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanupScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<ParticleSystem>() != null)
        {
            var PS = GetComponent<ParticleSystem>();
            Destroy(this.gameObject, PS.main.duration);
        }
        else
        {
            StartCoroutine(DestroyDelay());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
}
