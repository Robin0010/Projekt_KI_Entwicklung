using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Columns : MonoBehaviour
{
    [Header("References")]
    public GameObject columnPrefab;
    public GameObject columnParent;
    public GameObject topColumn;
    public GameObject bottomColumn;
    
    [Header("Settings")]
    public float spawnDistance;

    public float posTop = 0;
    public Vector3 posBottom;

    private bool isActivated = false;

    private void Awake()
    {
        posTop = topColumn.transform.position.y;
        posBottom = bottomColumn.transform.position;
    }
    private void Start()
    {
        columnParent = GameObject.Find("ColumnsParent");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject nextColumn = Instantiate(columnPrefab, new Vector3(transform.position.x + spawnDistance, Random.Range(2f, 5f), 0), Quaternion.identity);
                nextColumn.transform.parent = columnParent.transform;
                isActivated = true;
            }
        }
    }
}
