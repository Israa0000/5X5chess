using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationCubes : MonoBehaviour
{

    [SerializeField] Transform Cube0;
    [SerializeField] Transform Cube1;
    [SerializeField] Transform Cube2;

    private void Start()
    {
        StartCoroutine(cube0Rotate());
        StartCoroutine(cube1Rotate());
        StartCoroutine(cube2Rotate());
    }


    IEnumerator cube0Rotate()
    {
        while (true)
        {
            Cube0.Rotate(Vector3.up * 45 * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator cube1Rotate()
    {
        float time = 0;
        while (true)
        {
            Vector3 eje = new Vector3(Mathf.Sin(time), Mathf.Cos(time), Mathf.Sin(time * 2));
            Cube1.Rotate(eje * 90 * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator cube2Rotate()
    {
        while (true)
        {
            float anguloX = Mathf.Sin(Time.time * 2f) * 45f;
            float anguloZ = Mathf.Cos(Time.time * 1.5f) * 30f;
            Cube2.localRotation = Quaternion.Euler(anguloX, 0, anguloZ);
            yield return null;
        }
    }
}
