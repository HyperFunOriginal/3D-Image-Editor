using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformNodeObject : MonoBehaviour
{
    public GameObject x, y, z;
    Camera cameraGO;
    Ray tempRay;
    Vector3 hitPosition;
    GameObject holding;

    Vector3 oldXScale;
    Vector3 oldYScale;
    Vector3 oldZScale;
    Vector3 oldScale;
    Vector3 oldPosition;
    Vector3 oldRotation;
    float accumulatedRotation;

    // Start is called before the first frame update
    void Start()
    {
        cameraGO = FindObjectOfType<Camera>();
        oldXScale = x.transform.localScale;
        oldYScale = y.transform.localScale;
        oldZScale = z.transform.localScale;
        oldPosition = transform.position;
        oldScale = transform.localScale;
        oldRotation = transform.eulerAngles;
        accumulatedRotation = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            tempRay = cameraGO.ScreenPointToRay(Input.mousePosition);
            if (holding == null)
            {
                if (Physics.Raycast(tempRay, out RaycastHit info, 100f))
                {
                    if (info.collider.gameObject == x)
                        holding = x;
                    if (info.collider.gameObject == y)
                        holding = y;
                    if (info.collider.gameObject == z)
                        holding = z;
                    hitPosition = info.point - holding.transform.position;

                    oldXScale = x.transform.localScale;
                    oldYScale = y.transform.localScale;
                    oldZScale = z.transform.localScale;
                    oldPosition = transform.position;
                    oldScale = transform.localScale;
                    oldRotation = transform.eulerAngles;
                    accumulatedRotation = 0f;
                }
            }
            else
            {
                Vector3 rayDir = holding.transform.forward.normalized;
                Vector3 targetTransform = transform.position;
                for (int i = 0; i < 10; i++)
                {
                    targetTransform = tempRay.origin + Vector3.Dot(targetTransform - tempRay.origin, tempRay.direction) * tempRay.direction;
                    targetTransform = transform.position + Vector3.Dot(targetTransform - transform.position, rayDir) * rayDir;
                }
                targetTransform -= rayDir * Vector3.Dot(hitPosition, rayDir);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    float newScale = Vector3.Dot(targetTransform - transform.position + hitPosition, rayDir) / Vector3.Dot(hitPosition, rayDir);
                    hitPosition += rayDir * Vector3.Dot(hitPosition, rayDir) * (newScale - 1f);
                    if (holding == x)
                    {
                        transform.localScale = new Vector3(newScale * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                        newScale = Mathf.Abs(newScale);
                        x.transform.localScale = new Vector3(x.transform.localScale.x, x.transform.localScale.y, x.transform.localScale.z / newScale);
                        y.transform.localScale = new Vector3(y.transform.localScale.x / newScale, y.transform.localScale.y, y.transform.localScale.z);
                        z.transform.localScale = new Vector3(z.transform.localScale.x / newScale, z.transform.localScale.y, z.transform.localScale.z);
                    }
                    if (holding == y)
                    {
                        transform.localScale = new Vector3(transform.localScale.x, newScale * transform.localScale.y, transform.localScale.z);
                        newScale = Mathf.Abs(newScale);
                        x.transform.localScale = new Vector3(x.transform.localScale.x, x.transform.localScale.y / newScale, x.transform.localScale.z);
                        y.transform.localScale = new Vector3(y.transform.localScale.x, y.transform.localScale.y, y.transform.localScale.z / newScale);
                        z.transform.localScale = new Vector3(z.transform.localScale.x, z.transform.localScale.y / newScale, z.transform.localScale.z);
                    }
                    if (holding == z)
                    {
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newScale * transform.localScale.z);
                        newScale = Mathf.Abs(newScale);
                        x.transform.localScale = new Vector3(x.transform.localScale.x / newScale, x.transform.localScale.y, x.transform.localScale.z);
                        y.transform.localScale = new Vector3(y.transform.localScale.x, y.transform.localScale.y / newScale, y.transform.localScale.z);
                        z.transform.localScale = new Vector3(z.transform.localScale.x, z.transform.localScale.y, z.transform.localScale.z / newScale);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    float rotation = Vector3.Dot(targetTransform - transform.position, rayDir) - accumulatedRotation;
                    accumulatedRotation += rotation;
                    transform.rotation = Quaternion.AngleAxis(rotation * 45f, rayDir.normalized) * transform.rotation;
                }
                else
                    transform.position = (transform.position + targetTransform * .2f) / 1.2f;
            }
        }
        else
        {
            holding = null;
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                x.transform.localScale = oldXScale;
                y.transform.localScale = oldYScale;
                z.transform.localScale = oldZScale;
                transform.position = oldPosition;
                transform.localScale = oldScale;
                transform.eulerAngles = oldRotation;
                accumulatedRotation = 0f;
            }
        }
    }
}
