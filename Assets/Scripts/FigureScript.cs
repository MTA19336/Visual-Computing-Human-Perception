using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureScript : MonoBehaviour
{
    GameObject interfaceController;
    InterfaceController ICscript;
    ShowModelController modelsScript;
    private List<GameObject> models;


    public int id;
    public int assignedModelId;
    public string assignedModelName;
    public bool hasModelAssigned = false;
    bool gotId;
    bool claimed;
    string claimedByName;
    int modelsLoaded = 0;


    // Start is called before the first frame update
    void Start()
    {
        models = new List<GameObject>();
        interfaceController = GameObject.Find("InterfaceController");
        modelsScript = GameObject.Find("SwapableModels").GetComponent<ShowModelController>();
        ICscript = interfaceController.GetComponent<InterfaceController>();

        foreach(var model in modelsScript.GetGameObjects())
        {
            GameObject newModel = UnityEngine.Object.Instantiate(model, gameObject.GetComponent<Transform>());
            models.Add(newModel);
            RotateModel script;
            script = newModel.GetComponent<RotateModel>();
            script.rotate = false;
            script.scale = 0.1f;
            script.scaleThis();
            script.id = modelsLoaded++;
            newModel.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            newModel.SetActive(false);
        }

        gotId = false;
        claimed = false;
        claimedByName = "-";
        assignedModelName = "-";

    }

    // Update is called once per frame
    void Update()
    {
        if (!gotId)
        {
            id = ICscript.getNumberOfFigures();
            ICscript.addFigure(gameObject);
            gotId = true;
        }
    }

    public void assignModel(int modelId, string modelName)
    {
        assignedModelId = modelId;
        models[assignedModelId].SetActive(true);
        assignedModelName = modelName;
        hasModelAssigned = true;
        for (int i = 0; i < models.Count; i++)
        {
            if(models[i].GetComponent<RotateModel>().id != modelId)
            {
                models[i].SetActive(false);
            }
        }
    }

    public void unassignModel()
    {
        hasModelAssigned = false;
        models[assignedModelId].SetActive(false);
        assignedModelName = "-";
        assignedModelId = 0;
    }

    private void OnMouseDown()
    {
        ICscript.setSelectedFigure(id);
    }

    public bool isClaimed()
    {
        return claimed;
    }

    public void claim(string name)
    {
        claimed = true;
        claimedByName = name;
    }

    public void unclaim()
    {
        claimed = false;
        claimedByName = "-";
    }

    public string claimedBy()
    {
        return claimedByName;
    }

    public string getAssignedModelName()
    {
        return assignedModelName;
    }
}
