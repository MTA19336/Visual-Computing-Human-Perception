using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowModelUI : MonoBehaviour
{
    [SerializeField] private SelectModelButton buttonPrefab;

    private void Start()
    {
        var models = FindObjectOfType<ShowModelController>().GetModels();
        foreach (var model in models)
        {
            CreateButtonForModel(model);
        }
    }

    private void CreateButtonForModel(Transform model)
    {
        var button = Instantiate(buttonPrefab);
        
        button.transform.SetParent(this.transform);
        button.transform.localScale.Set(.5f, 2f, 2f);
        //button.transform.localRotation = Quaternion.identity;
        button.gameObject.transform.position = model.gameObject.transform.position;

        var controller = FindObjectOfType<ShowModelController>();
        var selected = FindObjectOfType<ShowSelectedModel>();
        button.Initialize(model, controller.EnableModels);
        button.Initialize(model, selected.EnableModels);
    }
}
