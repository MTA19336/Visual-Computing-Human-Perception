using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSelectedModel : MonoBehaviour
{
    private List<Transform> models;
    public int selectedModel = 0;
    Text selectedModelText;


    private void Awake()
    {
        models = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var model = transform.GetChild(i);
            models.Add(model);

            model.gameObject.SetActive(i == 0);
        }

        selectedModelText = GameObject.Find("ModelNameText").GetComponent<Text>();
        selectedModelText.text = models[selectedModel].gameObject.name;
    }

    public void EnableModels(int id)
    {
        selectedModel = id;
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].gameObject.GetComponent<SelectedModelScript>().id == id)
            {
                models[i].gameObject.SetActive(true);
                selectedModelText.text = models[i].gameObject.name;
            }
            else models[i].gameObject.SetActive(false);
        }
    }

    public string getSelectedModelName()
    {
        return models[selectedModel].gameObject.name;
    }

    public void rotateSelectedModel(float speed)
    {
        models[selectedModel].transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }

    public List<Transform> GetModels()
    {
        return new List<Transform>(models);
    }
}
