using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeuralNetBird))]
public class Bird : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 speed = new Vector2(3, 3);
    public LayerMask playerLayer;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 5f;
    public float maxFitness = 1000f;

    [Header("Network Options")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    public float timeSinceStart = 0f;

    public NeuralNetBird network;
    public Background[] backgrounds;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;

    private Vector3 startPosition, startRotation;
    private float aSensor, bSensor, cSensor;

    private NNetVisualiser visualiser;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NeuralNetBird>();
        backgrounds = GameObject.FindObjectsOfType<Background>();
        visualiser = GameObject.FindObjectOfType<NNetVisualiser>();

        visualiser.currentNet = GetComponent<NeuralNetBird>();
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        InputSensors();

        float _decisionA, _decisionB;

        (_decisionA, _decisionB) = network.RunNetwork(aSensor, bSensor, cSensor);

        float[] _decisions = new float[2];

        _decisions[0] = _decisionA;
        _decisions[1] = _decisionB;

        Move(_decisions);

        timeSinceStart += Time.deltaTime;

        CalculateFitness();

        //transform.Rotate(new Vector3(0, 0, -2));
    }

    private void Move(float[] _decisions)
    {
        float max = 0;
        int maxIndex = 0;

        for (int i = 0; i < _decisions.Length; i++)
        {
            if (max < _decisions[i])
            {
                max = _decisions[i];
                maxIndex = i;
            }
        }

        // maxIndex == 0 --> Jump
        // maxIndex == 1 --> Don't Jump

        if (maxIndex == 0)
        {
            rb.velocity = speed;
            //transform.eulerAngles = new Vector3(0, 0, 30);
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }


    private void Death()
    {
        anim.SetTrigger("Reset");
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].ResetBackground();
        }
        GameObject.FindObjectOfType<GeneticBird>().Death(overallFitness, network);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Death();
        isDead = true;
    }

    public void ResetWithNetwork(NeuralNetBird _net)
    {
        network = _net;
        visualiser.currentNet = _net;
        Reset();
    }

    public void Reset()
    {
        rb.velocity = new Vector2(0, 0);
        timeSinceStart = 0f;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        anim.SetTrigger("Reset");
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].ResetBackground();
        }
    }

    private void CalculateFitness()
    {

        overallFitness = transform.position.x * 10;

        if (timeSinceStart > 360)
        {
            //Save to a JSON File
            Death();
        }

        if (transform.position.y > 9 || transform.position.y < 0.3f)
        {
            Death();
        }

    }

    private void InputSensors()
    {
        aSensor = transform.position.y / 10;
        Vector3 a = new Vector3(1, 0, 0);

        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, a, Mathf.Infinity, ~playerLayer);

        if (hit2D == true)
        {
            if (hit2D.collider.gameObject != null)
            {
                if (hit2D.collider.gameObject.CompareTag("Column"))
                {
                    bSensor = (hit2D.collider.gameObject.GetComponent<Columns>().posBottom.x - transform.position.x) / 10;
                    cSensor = (hit2D.collider.gameObject.GetComponent<Columns>().posBottom.y) / 10;

                    Debug.DrawLine(transform.position, hit2D.point, Color.red);
                }
            }
        }
    }
}
