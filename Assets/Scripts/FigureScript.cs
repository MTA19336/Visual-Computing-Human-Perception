using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureScript : MonoBehaviour
{
    GameObject interfaceController;
    InterfaceController ICscript;


    public int id;
    bool gotId;
    bool claimed;
    string claimedByName;


    // Start is called before the first frame update
    void Start()
    {
        interfaceController = GameObject.Find("InterfaceController");
        ICscript = interfaceController.GetComponent<InterfaceController>();
        gotId = false;
        claimed = false;
        claimedByName = "-";

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
}
