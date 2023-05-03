using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    Animator anim;
   [HideInInspector] public CharacterController controller;
    float defmoveSpeed;
    float moveSpeed = 3f;
    float jumpForce = 0.7f;
    float gravity = -9.81f;
  
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    public float attackRadius = 2f;
    public Transform attackPos;
    Vector3 velocity;
    Vector3 movement;
    Transform enemy;

    public GameObject bulletPrefab;
    public GameObject muzzleFlash;

    bool canShoot = true;

    AudioSource audioSrcjump;
    AudioSource audioSrcshit;
    AudioSource audioSrcshoot;
    AudioSource audioSrcheal;

    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip healClip;

    // Start is called before the first frame update
    void Start()
    {
        defmoveSpeed = moveSpeed;
        currentHealth = maxHealth;
       
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        //each audio clips should have its own audio source for a better audio plays, if not then the clips will not play all at once.
        audioSrcjump = gameObject.AddComponent<AudioSource>();
        audioSrcshit = gameObject.AddComponent<AudioSource>();
        audioSrcshoot = gameObject.AddComponent<AudioSource>();
        audioSrcheal = gameObject.AddComponent<AudioSource>();

       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Movement();
        Shoot();
        DisplayHealth();

        if (!controller.isGrounded)
        {
            controller.stepOffset = 0.00f;
        }
        else
        {
            controller.stepOffset = 0.29f;
        }
    }

    private void Movement()
    {
        bool spaceBar = Input.GetKey(KeyCode.Space);
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, 0.0f).normalized;
        if(movement.x != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.4f); //the higher the value the faster the rotation (0.01 - 1.00)
            anim.SetBool("isRunning", true);
        }

        else
        {
            anim.SetBool("isRunning", false);
        }

        if (controller.isGrounded)
        {
            moveSpeed = defmoveSpeed;
            velocity.y = -0.5f;
            if (spaceBar && controller.isGrounded)
            {
                JumpSound();
                velocity.y += Mathf.Sqrt(jumpForce * -5.0f * gravity);
            }
        }

        else
        {
            moveSpeed = 2.4f; // if in midair, slow movement speed
        }

        anim.SetBool("isJumping", !controller.isGrounded);
        controller.Move(movement * Time.deltaTime * moveSpeed);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Shoot()
    {
        bool shootKey = Input.GetKeyDown(KeyCode.J);
        if (shootKey && controller.isGrounded && canShoot)
        {
            ShoootSound();
            Instantiate(bulletPrefab, attackPos.position, attackPos.rotation);
            Instantiate(muzzleFlash, attackPos.position, attackPos.rotation);
            StartCoroutine(fireRate());
        }
    }

    IEnumerator fireRate()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.18f);
        canShoot = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HitSound();
        if (currentHealth <= 0)
        {
            healthText.text = $"Health:{0}";
            SceneManager.LoadScene(0);
            Destroy(gameObject);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HealUp(int amount)
    {
        currentHealth += amount;
        audioSrcheal.clip = healClip;
        audioSrcheal.volume = 0.4f;
        audioSrcheal.Play();
    }

    void DisplayHealth()
    {
        healthText.text = $"Health:{currentHealth}";
    }

    void HitSound()
    {
        audioSrcshit.clip = hitClip;
        audioSrcshit.volume = 0.4f;
        audioSrcshit.Play();
    }

    void JumpSound()
    {
        audioSrcjump.clip = jumpClip;
        audioSrcjump.volume = 0.4f;
        audioSrcjump.Play();
    } 
    
    void ShoootSound()
    {
        audioSrcshoot.clip = shootClip;
        audioSrcshoot.volume = 0.07f;
        audioSrcshoot.PlayOneShot(audioSrcshoot.clip);
    }
}
