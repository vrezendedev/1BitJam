using System;
using System.Collections;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public int rotateDegrees;
    [HideInInspector] public GameObject lightDirection;
    [HideInInspector] public bool isRotating;

    void OnEnable()
    {
        EventManager.PlayerRotateLight += RotateLight;
    }

    void OnDisable()
    {
        EventManager.PlayerRotateLight -= RotateLight;
    }

    void Awake()
    {
        lightDirection = GameObject.FindGameObjectWithTag("Sunlight");
    }

    private void RotateLight()
    {
        if (!isRotating)
        {
            isRotating = true;
            StopAllCoroutines();
            StartCoroutine(WhileRotating());
        }
    }

    private IEnumerator WhileRotating()
    {
        float portion = rotateDegrees / 10;
        float stack = 0;

        do
        {
            lightDirection.transform.Rotate(0, portion, 0, Space.World);
            stack += portion;
            yield return new WaitForSeconds(0.05f);
        } while (stack < rotateDegrees);

        isRotating = false;

        yield break;
    }
}
