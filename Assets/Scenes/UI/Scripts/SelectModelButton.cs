using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectModelButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform objectToShow;
    //private Action<Transform> clickAction;
    public Sprite uisprite;
    Sprite uispriteS;
    public int id;
    InterfaceController icscript;

    public void Initialize(Transform objectToShow, Sprite sprite, Sprite spriteS, int id)
    {
        this.objectToShow = objectToShow;
        //this.clickAction = clickAction;
        this.id = id;
        uisprite = sprite;
        uispriteS = spriteS;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleButtonClick);
        GetComponent<Image>().sprite = uisprite;
        icscript = GameObject.Find("InterfaceController").GetComponent<InterfaceController>();
    }

    private void HandleButtonClick()
    {
        var controller = FindObjectOfType<ShowSelectedModel>();
        controller.EnableModels(id);
        icscript.updateInfoPanelUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = uispriteS;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = uisprite;
    }
}

