using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectModelButton: MonoBehaviour
{
    private Transform objectToShow;
    private Action<Transform> clickAction;

    public void Initialize(Transform objectToShow, Action<Transform> clickAction)
    {
        this.objectToShow = objectToShow;
        this.clickAction = clickAction;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        clickAction(objectToShow);
    }
}

