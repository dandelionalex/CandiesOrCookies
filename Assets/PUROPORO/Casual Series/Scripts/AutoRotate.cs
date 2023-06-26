using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 speed = new Vector3(0, 48, 0);

    void Update()
    {
        transform.Rotate(
             speed.x * Time.deltaTime,
             speed.y * Time.deltaTime,
             speed.z * Time.deltaTime
        );
    }
}