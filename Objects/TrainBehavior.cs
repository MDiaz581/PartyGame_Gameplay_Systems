using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBehavior : MonoBehaviour
{
    public float timerMax;
    public float timerMin;
    public float timer;
    public float time;
    private Animator animator;
    public Canvas canvas;
    public AudioSource AS;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = Random.Range(timerMin, timerMax);

        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TrainIdle"))
        {           
            time += Time.deltaTime;

            if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
        if (time >= timer - 5f)
        {
            if(canvas != null)
            {
                canvas.enabled = true;
            }
        }
        if (time >= timer)
        {
            animator.SetTrigger("Move");
            timer = Random.Range(timerMin, timerMax);
            time = 0f;
        } 

    }
}
