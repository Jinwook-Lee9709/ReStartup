using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUi : MonoBehaviour
{
    private enum FocusingArea
    {
        Food = 0,
        Employee = 1,
        Supervise = 2
    }

    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<FocusingArea, Button> buttons;
    
    [SerializedDictionary] [SerializeField]
    private SerializedDictionary<FocusingArea, GameObject> panels;

    [SerializeField] private Button closeButton;

    public GameObject employeeScrollView;
    public GameObject foodScrollView;
    public GameObject RestaurantSupervisePanel;

    private FocusingArea currentFocus;

    private FocusingArea CurrentFocus
    {
        get => currentFocus;
        set
        {
            currentFocus = value;
            
            closeButton.transform.SetSiblingIndex((int)currentFocus);
            buttons[currentFocus].gameObject.SetActive(false);
            buttons.Where(x=>x.Key != currentFocus).ToList().ForEach(x=>x.Value.gameObject.SetActive(true));
            panels[currentFocus].SetActive(true);
            panels.Where(x=>x.Key != currentFocus).ToList().ForEach(x=>x.Value.SetActive(false));
        }
    }

    private void Start()
    {
        CurrentFocus = FocusingArea.Employee;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitUiUpgrade();
            }
        }
    }

    public void EmployeeScrollViewOpenButton()
    {
        CurrentFocus = FocusingArea.Employee;
    }

    public void FoodScrollViewOpenButton()
    {
        CurrentFocus = FocusingArea.Food;
    }

    public void RestaurantSupervisePanelOpenButton()
    {
        CurrentFocus = FocusingArea.Supervise;
    }
}