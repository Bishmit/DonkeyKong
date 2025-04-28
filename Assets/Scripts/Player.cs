using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
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
            else if (hit.layer == LayerMask.NameToLayer("Stair"))
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
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
            rigidbody.gravityScale = 0f; // Turn off gravity while climbing
        }
        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jumpStrength;
        }
        else
        {
            rigidbody.gravityScale = 1f;
            direction += Physics2D.gravity * Time.deltaTime;
        }

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (!climbing)
        {
            direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        }

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

    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex = (spriteIndex + 1) % runSprites.Length;
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

     private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();            
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            enabled = false;
             FindObjectOfType<GameManager>().LevelFailed();
        }
    }
}
