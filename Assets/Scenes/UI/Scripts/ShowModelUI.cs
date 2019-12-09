using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowModelUI : MonoBehaviour
{
    [SerializeField] private SelectModelButton buttonPrefab;
    public Sprite uisprite;
    public Sprite uispriteS;
    public float x;
    public float y;
    RectTransform rect;
    int i = 0;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        //rect.offsetMin = new Vector2(rect.offsetMin.x, -Screen.height+65.5f);
        var models = FindObjectOfType<ShowModelController>().GetModels();
        foreach (var model in models)
        {
            CreateButtonForModel(model, i);
            i++;
        }
    }

    private void CreateButtonForModel(Transform model, int id)
    {
        var button = Instantiate(buttonPrefab);
        
        button.transform.SetParent(this.transform);
        button.transform.localScale = new Vector2(x, y);
        //button.transform.localRotation = Quaternion.identity;
        button.gameObject.transform.position = model.gameObject.transform.position;

        var controller = FindObjectOfType<ShowModelController>();
        button.Initialize(model, uisprite, uispriteS, id);        
    }
}
