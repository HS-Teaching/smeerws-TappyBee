using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigidbodyBird;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    // Use this for initialization
    void Start () {
        rigidbodyBird = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0,0,-90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        rigidbodyBird.simulated = false;
        game = GameManager.Instance;
	}

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void Disable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbodyBird.velocity = Vector3.zero;
        rigidbodyBird.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update () {
        if (game.GameOver) return;


        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            //Time.timeScale += 1;
            transform.rotation = forwardRotation;
            rigidbodyBird.velocity = Vector3.zero;
            rigidbodyBird.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            //register a score event
            OnPlayerScored(); // event sent to GameManager;
            scoreAudio.Play();

            //play a sound
        }

        if (collision.gameObject.tag == "DeadZone")
        {
            //register a score event
            rigidbodyBird.simulated = false;
            OnPlayerDied();
            dieAudio.Play();
            //play a sound
        }
    }
}
