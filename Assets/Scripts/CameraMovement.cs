using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public Transform target;

    void Update()
    {
        transform.position = new Vector3(target.position.x + 2.5f, 3, -10);
    }
}
