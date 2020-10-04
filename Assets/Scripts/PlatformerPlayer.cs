using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerPlayer : MonoBehaviour
{
    public float speed = 5000.0f;
    public float jumpForce = 4.5f;
    public int maxScore = 5;

    private AudioSource _tickSource;
    private Rigidbody2D _body;
    private Animator _anim;

    private BoxCollider2D _box;
    private int score = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Cat")
        {
            score++;
            Destroy(collision.gameObject);
            _tickSource.Play();
        }
        else if (collision.gameObject.tag == "Respawn")
        {
            SceneManager.LoadScene("Scene");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>();
        _tickSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
        _body.velocity = movement;

        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

        bool grounded = false;

        if (hit != null)
        {
            grounded = true;
        }

        _body.gravityScale = grounded && deltaX == 0 ? 0 : 1;
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        MovingPlatform platform = null;
        if (hit != null)
        {
            platform = hit.GetComponent<MovingPlatform>();
        }
        if (platform != null)
        {
            transform.parent = platform.transform;
        }
        else
        {
            transform.parent = null;
        }

        _anim.SetFloat("speed", Mathf.Abs(deltaX));

        Vector3 pScale = Vector3.one;
        if(platform != null)
        {
            pScale = platform.transform.localScale;
        }
        if (deltaX != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(deltaX) / pScale.x, 1 / pScale.y, 1);
        }

        if (!Mathf.Approximately(deltaX, 0))
        {
            transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
        }
    }
}
