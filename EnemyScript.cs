using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyScript : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    public float chaseRadius = 10f;
    float defchaseR;
    public int maxHealth = 100;
    public int currentHealth;
    bool canAttack = true;

    AudioSource audioSrcslimeMove;
    AudioSource audioSrcslimeslimeHit;
    AudioSource audioSrcslimeslimeDeath;

    [SerializeField] AudioClip moveClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip deathClip;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        target = PlayerManager.instance.Player.transform;
        agent = GetComponent<NavMeshAgent>();   
        anim = GetComponent<Animator>();
        defchaseR = chaseRadius;
        audioSrcslimeMove = gameObject.AddComponent<AudioSource>();
        audioSrcslimeslimeHit = gameObject.AddComponent<AudioSource>();
        audioSrcslimeslimeDeath = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = 1.6f;
        agent.stoppingDistance = 1f;
        agent.radius = 0.012f;
        if(target != null)
        {
            ChasePlayer();
        }
        Die();
       
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        PlayerScript player = target.GetComponent<PlayerScript>();
        if (distance <= chaseRadius)
        {
            agent.SetDestination(target.position);
            FaceTarget();
            anim.SetBool("slimeMoving", true);
            agent.isStopped = false;      
            if (distance <= agent.stoppingDistance && canAttack && currentHealth > 0)
            {
               // agent.isStopped = true;
                player.TakeDamage(8);        
                StartCoroutine(attackCooldown());
            }
        }
        else
        {
            anim.SetBool("slimeMoving", false);
            agent.isStopped = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    void FaceTarget()  //to face the player 
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HitSound();
        StartCoroutine(expandChaseRadius());
    }



    void HitSound()
    {
        audioSrcslimeslimeHit.clip = hitClip;
        audioSrcslimeslimeHit.spatialBlend = 1f;
        audioSrcslimeslimeHit.Play();
    }  
    
    public void MoveSound()
    {
        audioSrcslimeMove.clip = moveClip;
        audioSrcslimeMove.spatialBlend = 1f;
       // audioSrcslimeMove.loop = true;
        audioSrcslimeMove.Play();
    } 
    
    public void DieSound ()
    {
        audioSrcslimeslimeDeath.clip = deathClip;
        audioSrcslimeslimeDeath.spatialBlend = 1f;
        audioSrcslimeslimeDeath.Play();
    }

    void Die()
    {
        if (currentHealth <= 0)
        {
            agent.isStopped = true;
            anim.SetTrigger("slimeDed");
            Destroy(gameObject, 0.8f);
        }
    }

    IEnumerator attackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(2.6f);
        canAttack = true;
    }

    IEnumerator expandChaseRadius()
    {
        chaseRadius = 9f;
        yield return new WaitForSeconds(5f);
        chaseRadius = defchaseR;
    }
}
