using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    public float damage = 10f;
    public float range = 100f;
    public float shootDelay = 0.3f;
    public float recoilAmount = 50f;

    float shootTimer;

    public GameObject barrel;
    public AudioClip shootSound;

    public Camera camera;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        if(info.IsName("M14Shoot")) animator.SetBool("Shoot", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if(shootTimer < shootDelay) shootTimer += Time.deltaTime;
    }
    
    void Shoot()
    {
        
        if(shootTimer < shootDelay) return;
        shootTimer = 0f;

        
        animator.SetBool("Shoot", true);

        RaycastHit hit;

        if(Physics.Raycast(barrel.transform.position, camera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
        }


        // firing sound
        audioSource.PlayOneShot(shootSound);
    }
}
