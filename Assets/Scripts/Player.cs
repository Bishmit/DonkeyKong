using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private CapsuleCollider2D capsuleCollider;

    private readonly Collider2D[] overlaps = new Collider2D[4]; 

    private Vector2 direction;

    private bool grounded; 
    private bool climbing; 

    public float moveSpeed = 1f;
    public float jumpStrength = 4f; 

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();  
    }

    private void CheckCollision()
    {
        grounded = false; 
        climbing = false; 

        float skinOffset = 0.1f; 
        Vector2 size = capsuleCollider.bounds.size; 
        size.y += skinOffset; 
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, overlaps);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = overlaps[i].gameObject; 

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f); 
                Physics2D.IgnoreCollision(overlaps[i], capsuleCollider, !grounded);
            }

            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true; 
            }
        }
    }

    private void Update()
    {
        CheckCollision(); 

        if (climbing)
        {
            // Disable gravity while climbing
            rigidbody.gravityScale = 0;

            // Only move vertically while on the ladder
            direction.y = Input.GetAxis("Vertical") * moveSpeed;

            // Restrict horizontal movement while climbing
            direction.x = 0f;
        }
        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jumpStrength; 
        }
        else
        {
            // Restore gravity when not climbing
            rigidbody.gravityScale = 1;

            direction += Physics2D.gravity * Time.deltaTime; 
        }

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f); 
        }

        // Horizontal movement
        if (!climbing)
        {
            direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        }

        // Flipping player based on movement direction
        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }
}
