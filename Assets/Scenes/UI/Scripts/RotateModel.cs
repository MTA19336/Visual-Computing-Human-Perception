using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    [SerializeField]        //Makes rotationSpeed editable in the editor
    private float rotationSpeed = 15f;
    public bool rotate;
    public float scale;
    public int id;

    private void Start()
    {
      
    }

    public void scaleThis()
    {
        gameObject.GetComponent<Transform>().localScale = gameObject.GetComponent<Transform>().localScale * scale;
    }


    // Update is called once per frame
    private void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        }
    }
}
