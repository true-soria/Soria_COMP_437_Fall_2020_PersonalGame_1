using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlatformerPlayer : MonoBehaviour
{
    public static int Score;
    public static bool InMyZone;
    
    public float speed = 1500.0f;
    public float jumpForce = 4.5f;
    public float instantGravityForce = 50f;
    public int jumpDelayFrames = 7;
    public int extraJumps = 1;

    private Rigidbody2D _body;
    private Animator _anim;

    private BoxCollider2D _box;
    private int _jumpDelay = 0;
    private int _jumpsLeft = 0;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            InMyZone = true;
        }
        else if (collision.gameObject.CompareTag("Respawn"))
        {
            SceneManager.LoadScene("BeepBlockSkyway");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            InMyZone = false;
        }
    }

    public static void AddScore()
    {
        Score++;
    }

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>();
        Score = 0;
        InMyZone = false;

    }

    // Update is called once per frame
    void Update()
    {
        _jumpDelay--;
        float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
        _body.velocity = movement;

        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

        bool grounded = hit != null && !hit.isTrigger;
        _body.gravityScale = grounded ? 0 : 1;
        
        // Jump mechanic
        if (Input.GetKeyDown(KeyCode.Space) && _jumpDelay <= 0)
        {
            if (grounded)
            {
                _body.velocity = Vector2.zero;
                _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpDelay = jumpDelayFrames;
                _jumpsLeft = extraJumps;
            }
            else if (_jumpsLeft > 0)
            {
                _body.velocity = Vector2.zero;
                _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpsLeft--;
                _jumpDelay = jumpDelayFrames;                
            }
        }

        // Insta-drop mechanic
        if (Input.GetKeyDown(KeyCode.LeftControl) && !grounded)
        {
            _body.AddForce(Vector2.down * instantGravityForce, ForceMode2D.Impulse);
        }

        MovingPlatform platform = grounded ? hit.GetComponent<MovingPlatform>() : null;
        transform.parent = platform != null ? platform.transform : null;

        _anim.SetFloat("speed", Mathf.Abs(deltaX));

        Vector3 pScale = platform != null ? platform.transform.localScale : Vector3.one;
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
