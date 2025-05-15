using UnityEngine;
using System.Collections;

public class BanditPlayer2 : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    
    public float m_reboundForce = 15.0f; 

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool HitReceived;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

    void Update()
    {
        // Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            inputX = 1f;

        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // ? No se mueve si est� golpeado
        if (!HitReceived)
        {
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
        }

        m_animator.SetFloat("AirSpeed", m_body2d.linearVelocity.y);

        // -- Handle Animations --
        m_animator.SetBool("HitReceived", HitReceived);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_animator.SetTrigger("Attack");
        }
        else if (Input.GetKeyDown("f"))
        {
            m_combatIdle = !m_combatIdle;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_animator.SetInteger("AnimState", 2);
        }
        else if (m_combatIdle)
        {
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_animator.SetInteger("AnimState", 0);
        }
    }

    public void Damage2(Vector2 direction, int damage)
    {
        if (!HitReceived)
        {
            HitReceived = true;

            // ? Rebote horizontal dependiendo del lado del atacante
            float xDir = transform.position.x < direction.x ? -1f : 1f;
            Vector2 rebound = new Vector2(xDir, 0f);
            m_body2d.AddForce(rebound * m_reboundForce, ForceMode2D.Impulse);
        }
    }

    public void DeactivateDamage2()
    {
        HitReceived = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            BanditPlayer1 banditPlayer = collision.gameObject.GetComponentInParent<BanditPlayer1>();

            if (banditPlayer != null)
            {
                Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
                Damage2(directionDamage, 1);
            }
            else
            {
                Debug.LogWarning("El objeto con etiqueta 'Sword' no tiene el componente BanditPlayer1.");
            }
        }
    }
}
