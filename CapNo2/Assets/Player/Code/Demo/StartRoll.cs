using UnityEngine;

public class StartRoll : MonoBehaviour
{
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;

    public float moveSpeed = 5.0f; // 좌우 이동 속도


    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator.SetTrigger("Jump");
    }

    void Update()
    {
        HandleMovement();
    }



    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 velocity = m_rigidbody.velocity;
        velocity.x = moveInput * moveSpeed;
        m_rigidbody.velocity = velocity;

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
