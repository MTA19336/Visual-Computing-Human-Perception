using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowModelController : MonoBehaviour
{
    private List<Transform> models;

    private void Awake()
    {
        models = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var model = transform.GetChild(i);
            models.Add(model);

            model.gameObject.SetActive(true);
        }
    }

    public void EnableModels()
    {
        for(int i = 0; i < models.Count; i++)
        {
            models[i].gameObject.SetActive(true);
        }
    }

    public List<Transform> GetModels()
    {
        return new List<Transform>(models);
    }

    public List<GameObject> GetGameObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        foreach(Transform model in models)
        {
            gameObjects.Add(model.gameObject);
        }
        return gameObjects;
    }
}
