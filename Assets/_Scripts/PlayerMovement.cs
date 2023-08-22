using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 0f;
    [SerializeField] public float speedBoost = 0f;
    private float _initialSpeed = 0f;

    public float stamina = 5f;
    [HideInInspector] public float _initialStamina = 0f;

    [HideInInspector] public bool running = false;
    [HideInInspector] public bool recovering = false;

    private CameraRotation _playerCamera;

    public AudioSource AudioSource;
    public AudioClip[] clips;
    private int currentClip = 0;

    void Awake()
    {
        _playerCamera = Camera.main.GetComponent<CameraRotation>();
        _initialSpeed = _speed;
        _initialStamina = stamina;
    }

    void Update()
    {
        Move();
        Run();
    }

    void Move()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        this.transform.Translate(new Vector3(x, 0, z) * _speed * Time.deltaTime, Space.Self);
        this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
        this.transform.GetComponent<Rigidbody>().MoveRotation(_playerCamera.gameObject.transform.rotation);
        if (!AudioSource.isPlaying && (x > 0 || z > 0))
        {
            AudioSource.PlayOneShot(clips[currentClip]);
            currentClip++;
            if (currentClip == clips.Length)
                currentClip = 0;

        }
    }

    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0) && !running && stamina > 0)
        {
            _speed += _speed * speedBoost;
            running = true;
            recovering = false;
            StartCoroutine(WhileRunning());
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || stamina == 0 || (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0))
        {
            running = false;
            _speed = _initialSpeed;
        }

        if (!recovering && !running && stamina < _initialStamina)
        {
            recovering = true;
            StartCoroutine(WhileRecovering());
        }
    }


    private IEnumerator WhileRecovering()
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
            stamina += _initialStamina * 0.1f;
        } while (recovering && stamina < _initialStamina);

        recovering = false;

        yield break;
    }

    private IEnumerator WhileRunning()
    {
        do
        {
            stamina = stamina - (_initialStamina * 0.1f);
            stamina = Mathf.Clamp(stamina, 0.0f, _initialStamina);
            yield return new WaitForSeconds(0.5f);
        } while (running && stamina > 0 && (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0));

        yield break;
    }

}
