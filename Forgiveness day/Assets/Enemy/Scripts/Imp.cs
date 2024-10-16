using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : BaseAI
{
    [Header("Don't forget about Rigidbody")]
    [Header("FlyControl in percent of free space")]
    [SerializeField, Range(25,100)] float maxFlyHight;
    [SerializeField, Range(0,75)] float minFlyHight;
    [SerializeField, Tooltip("in seconds"), Range(0f,10f)] float moveTime;
    [SerializeField] float checkDistance;
    [SerializeField, Range(0.3f,5)] float minChangeHight = 0.2f;
    [SerializeField] LayerMask checkForFreeSpaceLayer;
    private Rigidbody _impBody;
    private SphereCollider _impCollider;
    private bool _isMoving;

    private void Awake()
    {
        _impBody = GetComponent<Rigidbody>();
        _impBody.isKinematic = true;
        _impCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        Flying();
    }

    protected void Flying()
    {
        if (!_isMoving) 
            ChooseNewHeight();
    }


    void ChooseNewHeight()
    {
        bool moveUp = Random.value > 0.5f;
        float freeSpaceAbove = GetFreeSpace(Vector3.up);
        float freeSpaceBelow = GetFreeSpace(Vector3.down);


        float allSpace = freeSpaceAbove + freeSpaceBelow;
        if (freeSpaceAbove - allSpace * maxFlyHight / 100 < 0)
        {
            moveUp = false;
        }
        if (freeSpaceBelow - allSpace * minFlyHight /100 < 0)
        {
            moveUp = true;
        }

        float targetOffsetAbove = Random.Range(minChangeHight, freeSpaceAbove - allSpace *(1-  maxFlyHight / 100));
        float targetOffsetBelow = Random.Range(minChangeHight, freeSpaceBelow - allSpace * minFlyHight / 100);

        if (targetOffsetAbove < minChangeHight && targetOffsetBelow >= minChangeHight)
        {
            moveUp = false;
        }
        else if (targetOffsetBelow < minChangeHight && targetOffsetAbove >= minChangeHight)
        {
            moveUp = true;
        }
        else if (targetOffsetAbove < minChangeHight && targetOffsetBelow < minChangeHight)
        {
            if (freeSpaceAbove < minChangeHight)
            {
                moveUp = false;
                targetOffsetBelow = minChangeHight;
            }
            else if (freeSpaceBelow < minChangeHight)
            {
                moveUp = true;
                targetOffsetAbove = minChangeHight;
            }
        }

        float chosenOffset = moveUp ? targetOffsetAbove : -targetOffsetBelow;

        if (Mathf.Abs(chosenOffset) < minChangeHight)
        {
            chosenOffset = minChangeHight * Mathf.Sign(chosenOffset);
        }
        Debug.Log(chosenOffset);

        Vector3 newTarget = transform.position + new Vector3(0, chosenOffset, 0);

        if (IsPositionFree(newTarget))
        {
            StartCoroutine(MoveToPosition(newTarget));
        }
        else
        {
            ChooseNewHeight();
        }
        
    }
    float GetFreeSpace(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, checkDistance, checkForFreeSpaceLayer))
        {
            return hit.distance;

        }
        else
        {
            return checkDistance;
        }
    }

    bool IsPositionFree(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, _impCollider.radius, checkForFreeSpaceLayer);
        return hits.Length == 0;
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        _isMoving = true;
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, target, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = target;
        _isMoving = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * GetFreeSpace(Vector3.up));
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * GetFreeSpace(Vector3.down));
        Gizmos.color = Color.blue;
        float allSpace = GetFreeSpace(Vector3.up) + GetFreeSpace(Vector3.down);
        Gizmos.DrawLine(transform.position + Vector3.down * (GetFreeSpace(Vector3.down) - allSpace * minFlyHight / 100),
            transform.position + Vector3.up * (GetFreeSpace(Vector3.up) - allSpace * (1 - maxFlyHight / 100)));
        Debug.Log( (allSpace* (1 - maxFlyHight /100)).ToString());
    }
}
