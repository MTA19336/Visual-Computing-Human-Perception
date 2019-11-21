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
    Button claimButton;
    RectTransform infoPanel;
    RectTransform canvasRectT;
    

    [SerializeField] public List<GameObject> figures;
    List<PlayerClass> players;
 
    

    bool figureIsSelected;
    int selectedFigure;
    string userName;
    Color32 green;
    Color32 red;


    // Start is called before the first frame update
    void Start()
    {
        //TESTING PURPOSES
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
        canvasRectT = GameObject.Find("UI_canvas").GetComponent<RectTransform>();
        claimButton = GameObject.Find("claimButton").GetComponent<Button>();
        claimButtonText = GameObject.Find("claimText").GetComponent<Text>();
        claimedByName = GameObject.Find("ClaimedByName").GetComponent<Text>();

        figureIsSelected = false;
        green = new Color32(0, 144, 7, 255);
        red = new Color32(219, 0, 0, 255);

        hideInfoPanel();
        
    }

    // Update is called once per frame
    void Update()
    {
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

    private void updateInfoPanelUI()
    {
        claimedByName.text = figure1.GetComponent<FigureScript>().claimedBy();
    }

    public void unselectFigure()
    {
        figureIsSelected = false;
        hideInfoPanel();
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

}

public class PlayerClass
{
    string name;

    public PlayerClass(string nme)
    {
        name = nme;
    }
}
