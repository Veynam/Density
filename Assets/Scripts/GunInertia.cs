using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInertia : MonoBehaviour
{
    public float amount; //(0.2)
    public float smoothAmount; //(5)

    Vector3 pos;

    void Start()
    {
        pos = transform.localPosition;
    }

    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;

        Vector3 finalPos = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + pos, Time.deltaTime * smoothAmount);
    }
}
