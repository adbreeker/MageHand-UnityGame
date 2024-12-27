using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticAttraction : MonoBehaviour
{
    GameObject _attractionTarget;
    GameObject _magneticFlyEffect;
    Coroutine _flyingCoroutine;

    public void Initialize(GameObject attractionTarget, GameObject magneticFlyEffect)
    {
        _attractionTarget = attractionTarget;
        _magneticFlyEffect = magneticFlyEffect;

        Rigidbody rb = GetComponent<Rigidbody>();
        ThrowObject to = GetComponent<ThrowObject>();

        if (rb != null)
        {
            Destroy(rb);
        }

        if (to != null)
        {
            Destroy(to);
        }

        _flyingCoroutine = StartCoroutine(MagneticFlyingCoroutine());
    }

    IEnumerator MagneticFlyingCoroutine()
    {
        bool onCircle = false;
        while (!onCircle)
        {
            // Calculate direction from object to player
            Vector3 directionToPlayer = transform.position - _attractionTarget.transform.position;
            directionToPlayer.y = 0f;  // Lock movement to the y-axis plane

            // Normalize the direction and scale by radius
            Vector3 targetPosition = _attractionTarget.transform.position + directionToPlayer.normalized * 1.5f;
            targetPosition.y = _attractionTarget.transform.position.y + 0.3f; // Maintain the object's current y position

            // Move the object to the nearest point on the circle
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * 10f);

            // Check if object is close enough to the circle
            if (transform.position == targetPosition)
            {
                onCircle = true;
                transform.SetParent(_attractionTarget.transform);
            }

            yield return new WaitForFixedUpdate();
        }

        while(true)
        {
            gameObject.transform.RotateAround(_attractionTarget.transform.position, new Vector3(0, 1, 0), 2f);
            yield return new WaitForFixedUpdate();
        }
    }

    public void OnPickUp()
    {
        StopCoroutine(_flyingCoroutine);
        Destroy(_magneticFlyEffect);
        Destroy(this);
    }
}
