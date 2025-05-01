using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PixelUiPlusDemo : MonoBehaviour
{

    public GameObject Acheivements;
    public GameObject ButtonList;
    public GameObject GradientBTN;
    public GameObject InfoItemsList;

    public GameObject MainDialog;
    public GameObject LevelDialog;
    public GameObject LoginUI;
    public GameObject LevelFailed;


    public GameObject LevelPassed;
    public GameObject PopUpWhite;
    public GameObject PopUpBlue;
    public GameObject PopUpyellow;

    public GameObject PopUpRED;
    public GameObject Rankings_Dialog;
    public GameObject SettingItemsList;


    public GameObject SHOP;



    public void OnClick_DemoButton(string itemName)
    {
        DisableAllUIElements();

        switch (itemName)
        {
            case "Acheivements":
                Acheivements.SetActive(true);
                break;

            case "Buttons List":
                ButtonList.SetActive(true);
                break;

            case "Gradient BTN":
                GradientBTN.SetActive(true);
                break;

            case "Info Items List":
                InfoItemsList.SetActive(true);
                break;

            case "Main Dialog":
                MainDialog.SetActive(true);
                break;

            case "Level Dialog":
                LevelDialog.SetActive(true);
                break;

            case "Login UI":
                LoginUI.SetActive(true);
                break;

            case "Level Failed":
                LevelFailed.SetActive(true);
                break;

            case "Level Passed":
                LevelPassed.SetActive(true);
                break;

            case "PopUp White":
                PopUpWhite.SetActive(true);
                break;

            case "PopUp Blue":
                PopUpBlue.SetActive(true);
                break;

            case "PopUp yellow":
                PopUpyellow.SetActive(true);
                break;

            case "PopUp RED":
                PopUpRED.SetActive(true);
                break;

            case "RANKINGS DIALOG":
                Rankings_Dialog.SetActive(true);
                break;

            case "Setting Items List":
                SettingItemsList.SetActive(true);
                break;


            case "SHOP":
                SHOP.SetActive(true);
                break;
        }
        Debug.Log(itemName);
    }

    public void DisableAllUIElements()
    {
        GameObject[] uiElements = {
            Acheivements, ButtonList, GradientBTN, InfoItemsList,
            MainDialog, LevelDialog, LoginUI, LevelFailed,
            LevelPassed, PopUpWhite, PopUpBlue, PopUpyellow,
            PopUpRED, Rankings_Dialog, SettingItemsList,
            SHOP
        };

        foreach (GameObject element in uiElements)
        {
            if (element != null)
            {
                element.SetActive(false);
            }
        }
    }

}
