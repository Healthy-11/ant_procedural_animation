using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolver otherFoot1 = default;
    [SerializeField] IKFootSolver otherFoot2 = default;
    float speed = 10;
    [SerializeField] float stepDistance = 0.5f;
    float stepLength = 0.1f;
    float stepHeight = 0.5f;
    [SerializeField] Vector3 footOffset = default;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    // Update is called once per frame

    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        

        Ray ray = new Ray(new Vector3(body.position.x, body.position.y + 1, body.position.z) + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10f, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot1.IsMoving() && !otherFoot2.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = info.normal;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }



}