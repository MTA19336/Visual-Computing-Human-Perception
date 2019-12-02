using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    [SerializeField]        //Makes rotationSpeed editable in the editor
    private float rotationSpeed = 15f;

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}
