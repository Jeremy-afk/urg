using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;

    [SerializeField]
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(player.transform.position.x - transform.position.x, Mathf.Abs(player.transform.position.y - transform.position.y), player.transform.position.z - transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x - offset.x, offset.y , player.transform.position.z -  offset.z);
    }
}
