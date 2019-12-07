using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{

    GameObject figure1;
    Text infoName;
    Text claimButtonText;
    Text claimedByName;
    Text assignedModelName;
    Text fpsText;
    Button claimButton;
    Button assignModelButton;
    RectTransform infoPanel;
    RectTransform canvasRectT;
    ShowSelectedModel selectedModelScript;
    

    [SerializeField] public List<GameObject> figures;
    List<PlayerClass> players;
 
    

    bool figureIsSelected;
    bool showFps = true;
    bool firstFrame = true;
    int selectedFigure;
    string userName;
    public float averageFps;
    int totalFrames = 0;
    float current;
    Color32 green;
    Color32 red;
    Color32 gray;


    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerClass>();
        userName = "Rick";
        for(int i = 0; i < 4; i++)
        {
            players.Add(new PlayerClass("Jimmy" + i.ToString()));
        }
        //TESTING ENDS

        figure1 = GameObject.Find("Figure1");
        infoName = GameObject.Find("FigureText").GetComponent<Text>();
        infoPanel = GameObject.Find("FigureInformationPanel").GetComponent<RectTransform>();
        canvasRectT = GameObject.Find("Canvas").GetComponent<RectTransform>();
        claimButton = GameObject.Find("claimButton").GetComponent<Button>();
        claimButtonText = GameObject.Find("claimText").GetComponent<Text>();
        claimedByName = GameObject.Find("ClaimedByName").GetComponent<Text>();
        assignedModelName = GameObject.Find("AssignedModelText").GetComponent<Text>();
        fpsText = GameObject.Find("FPStext").GetComponent<Text>();
        assignModelButton = GameObject.Find("AssignModelButton").GetComponent<Button>();
        selectedModelScript = GameObject.Find("ChosenModel").GetComponent<ShowSelectedModel>();

        figureIsSelected = false;
        green = new Color32(0, 144, 7, 255);
        red = new Color32(219, 0, 0, 255);
        gray = new Color32(125, 125, 125, 255);

        hideInfoPanel();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showFps && !firstFrame)
        {
            current = (int)(1f / Time.unscaledDeltaTime);
            updateAverageFps(current);
            fpsText.text = "FPS: " + current.ToString() + "\n" + "Average:  " + averageFps.ToString();
        }

        if (firstFrame) { firstFrame = false; }

        if (figureIsSelected)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, figure1.GetComponent<Transform>().position);

            infoPanel.anchoredPosition = screenPoint;
        }


    }

    public void addFigure(GameObject figure)
    {
        figures.Add(figure);
    }

    public int getNumberOfFigures()
    {
        return figures.Count;
    }

    private void hideInfoPanel()
    {
        infoPanel.transform.localScale = new Vector3(0, 0, 0);
    }

    private void showInfoPanel()
    {
        infoPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    private void setClaimButtonToClaimed()
    {
        claimButtonText.text = "Unclaim";
        claimButton.GetComponent<Image>().color = red;
    }

    private void setClaimButtonToUnclaimed()
    {
        claimButtonText.text = "Claim";
        claimButton.GetComponent<Image>().color = green;
    }

    private void setClaimButtonToOtherClaimed()
    {

    }//NOTHING HERE YET

    public void updateInfoPanelUI()
    {
        claimedByName.text = figure1.GetComponent<FigureScript>().claimedBy();
        assignedModelName.text = figure1.GetComponent<FigureScript>().getAssignedModelName();

        if (figureIsSelected)
        {
            if (figure1.GetComponent<FigureScript>().hasModelAssigned && selectedModelScript.selectedModel == figure1.GetComponent<FigureScript>().assignedModelId)
            {
                assignModelButton.GetComponent<Image>().color = red;
                assignModelButton.GetComponentInChildren<Text>().text = "Unassign model";
            }
            else
            {
                assignModelButton.GetComponent<Image>().color = green;
                assignModelButton.GetComponentInChildren<Text>().text = "Assign model to Figure " + selectedFigure.ToString();
            }
        } else
        {
            assignModelButton.GetComponent<Image>().color = gray;
            assignModelButton.GetComponentInChildren<Text>().text = "Select a figure to assign model";

        }
    }

    public void unselectFigure()
    {
        figureIsSelected = false;
        hideInfoPanel();
        updateInfoPanelUI();
    }

    public void setSelectedFigure(int id)
    {
        figureIsSelected = true;
        selectedFigure = id;
        figure1 = figures[selectedFigure];
        infoName.text = "Figure " + selectedFigure.ToString();

        if (figure1.GetComponent<FigureScript>().isClaimed())
        {
            setClaimButtonToClaimed();
        }
        else
        {
            setClaimButtonToUnclaimed();
        }

       
        updateInfoPanelUI();
        showInfoPanel();
    }

    private void updateAverageFps(float newFps)
    {
        ++totalFrames;
        averageFps += (newFps - averageFps) / totalFrames;
    }

    public void refreshFigureList()
    {
        //refreshes the list of tracked figures? not sure if needed
    }

    //BUTTON CONTROLS
    public void leaveOnClick()
    {
        //leave the session
    }

    public void claimOnClick()
    {
        if (figure1.GetComponent<FigureScript>().isClaimed())
        {
            figure1.GetComponent<FigureScript>().unclaim();
            setClaimButtonToUnclaimed();
        } else
        {
            figure1.GetComponent<FigureScript>().claim(userName);
            setClaimButtonToClaimed();
        }
        updateInfoPanelUI();
    }

    public void assignButtonOnClick()
    {
        if (figureIsSelected && figure1.GetComponent<FigureScript>().hasModelAssigned && selectedModelScript.selectedModel == figure1.GetComponent<FigureScript>().assignedModelId)
        {
            figure1.GetComponent<FigureScript>().unassignModel();
        } else if (figureIsSelected)
        {
            figure1.GetComponent<FigureScript>().assignModel(selectedModelScript.selectedModel, selectedModelScript.getSelectedModelName());
            assignedModelName.text = figure1.GetComponent<FigureScript>().getAssignedModelName();
        }

        updateInfoPanelUI();
    }

}

public class PlayerClass
{
    string name;

    public PlayerClass(string nme)
    {
        name = nme;
    }
}
