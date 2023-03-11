using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualiserCamera : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 targetRotation;
    float targetRadius;
    public GameObject plane;
    GameObject cameraGO;
    // Start is called before the first frame update
    void Start()
    {
        cameraGO = transform.GetChild(0).gameObject;
        targetRadius = 8f;
        targetRotation = new Vector3(30f, -135f);
    }

    // Update is called once per frame
    void Update()
    {
        cameraGO.transform.localPosition = Vector3.forward * (cameraGO.gameObject.transform.localPosition.z - targetRadius * .07f) / 1.07f;
        targetRadius = Mathf.Clamp(targetRadius * Mathf.Exp(-Input.mouseScrollDelta.y * .1f), 0.1f, 100f);

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - mousePosition;
            targetRotation = Quaternion.Euler(targetRotation + new Vector3(-delta.y, delta.x, 0) * .2f).eulerAngles;
        }

        targetRotation.x = (targetRotation.x + 180f) % 360f - 180f;
        targetRotation.y = (targetRotation.y + 180f) % 360f - 180f;
        targetRotation.z = (targetRotation.z + 180f) % 360f - 180f;
        targetRotation.z *= 0.96f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), 0.075f);
        mousePosition = Input.mousePosition;

        plane.transform.position = new Vector3(cameraGO.transform.position.x, -.075f, cameraGO.transform.position.z);

        if (Input.GetKeyDown(KeyCode.X))
            targetRotation = Vector3.down * 90f;
        if (Input.GetKeyDown(KeyCode.Y))
            targetRotation = Vector3.right * 90f;
        if (Input.GetKeyDown(KeyCode.Z) && !Input.GetKey(KeyCode.LeftControl))
            targetRotation = Vector3.zero;
    }
}
