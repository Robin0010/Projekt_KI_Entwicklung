using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NeuralNet))]
public class CarController : MonoBehaviour
{
    public Text fitnessText;
    public Text speedText;

    private Vector3 startPosition, startRotation;
    public NeuralNet network;

    [Range(-1f, 1f)]
    public float a, t;

    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;
    public float maxFitness = 1000f;

    [Header("Network Options")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor;

    private float deathTimer = 0.5f;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NeuralNet>();
    }

    private void Update()
    {
        int _fitness = Mathf.RoundToInt(overallFitness);
        fitnessText.text = "Current Fitness: " + _fitness;

        int _speed = Mathf.RoundToInt(a * 100);
        speedText.text = _speed + " km/h";

        if (deathTimer > 0)
        {
            deathTimer -= Time.deltaTime;
        }
    }

    public void ResetWithNetwork(NeuralNet _net)
    {
        network = _net;
        Reset();
    }

    public void Reset()
    {
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Death();
    }

    private void FixedUpdate()
    {
        InputSensors();
        lastPosition = transform.position;


        (a, t) = network.RunNetwork(aSensor, bSensor, cSensor);


        MoveCar(a, t);

        timeSinceStart += Time.deltaTime;

        CalculateFitness();
    }

    private void Death()
    {
        if (deathTimer <= 0)
        {
            GameObject.FindObjectOfType<Genetic>().Death(overallFitness, network);
            deathTimer = 0.5f;
        }
    }

    private void CalculateFitness()
    {
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        overallFitness = (totalDistanceTravelled * distanceMultiplier) + (avgSpeed * avgSpeedMultiplier) + (((aSensor + bSensor + cSensor) / 3) * sensorMultiplier);

        if (timeSinceStart > 20 && overallFitness < 40)
        {
            Death();
        }

        if (timeSinceStart > 360)
        {
            //Save to a JSON File
            Death();
        }
    }

    private void InputSensors()
    {
        Vector3 a = (transform.forward + transform.right);
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            aSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = b;

        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = c;

        if (Physics.Raycast(r, out hit))
        {
            cSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }
    }

    private Vector3 inp;
    public void MoveCar(float v, float h)
    {
        inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;

        transform.eulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);

        transform.position = new Vector3(transform.position.x, 0.74f, transform.position.z);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
