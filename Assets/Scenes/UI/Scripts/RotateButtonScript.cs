using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
                                               

public class RotateButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public Sprite uisprite;
    public Sprite uispriteS;
    public bool rotateLeft;
    public float rotationSpeed = 15f;
    private bool pointerDown = false;
    ShowSelectedModel selectedModelScript;


    // Start is called before the first frame update
    void Start()
    {
        if (rotateLeft == false)
        {
            rotationSpeed = -rotationSpeed;
        }
        selectedModelScript = GameObject.Find("ChosenModel").GetComponent<ShowSelectedModel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerDown)
        {
            selectedModelScript.rotateSelectedModel(rotationSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = uispriteS;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = uisprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }
}
