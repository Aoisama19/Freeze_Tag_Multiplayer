using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Variables
    public Transform InstentiatedChracters;
    public ScrollRect MapScrollBar;
    public float ScrollNavigateSpeed;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI CityText;
    public string Mode;
    public string City;
    public string Chracter;
    public float[] ScrollStoppingPoints;
    public string[] AudioLanguage;
    public GameObject ScrollRightBtn;
    public GameObject ScrollLeftBtn;
    public TextMeshProUGUI ScrollCountText;
    public TextMeshProUGUI LanguageText;
    public GameObject MessageTxt;
    public GameObject CharactersBtn;

    [HideInInspector]
    public bool PlayerTypeSelected;

    [Header("City Scene Indexes")]
    [Tooltip("Scene Index For Karachi")]
    public int Karachi;
    [Tooltip("Scene Index For Lahore")]
    public int Lahore;
    [Tooltip("Scene Index For Islamabad")]
    public int Islamabad;
    [Tooltip("Scene Index For Village")]
    public int Village;
    [Tooltip("Scene Index For Faislabad")]
    public int Faislabad;
    #endregion

    #region Private Variables
    int CurruntPoint;
    int CurruntLanguage;
    bool ScrollLeft;
    bool ScrollRight;
    [HideInInspector]
    public bool WrongSelect;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurruntPoint = 0;
        WrongSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        CityText.text = City;
        if (ScrollRight)
        {
            if (MapScrollBar.horizontalNormalizedPosition <= ScrollStoppingPoints[CurruntPoint])
            {
                MapScrollBar.horizontalNormalizedPosition += Time.deltaTime * ScrollNavigateSpeed;
            }
            else
            {
                ScrollRight = false;
            }
        }

        if (ScrollLeft)
        {
            if (MapScrollBar.horizontalNormalizedPosition >= ScrollStoppingPoints[CurruntPoint])
            {
                MapScrollBar.horizontalNormalizedPosition -= Time.deltaTime * ScrollNavigateSpeed;
            }
            else
            {
                ScrollLeft = false;
            }
        }
        if(MapScrollBar.horizontalNormalizedPosition <= ScrollStoppingPoints[0])
        {
            ScrollLeftBtn.SetActive(false);
            ScrollCountText.text = "1";
        }
        else
        {
            ScrollLeftBtn.SetActive(true);
        }

        if(MapScrollBar.horizontalNormalizedPosition >= ScrollStoppingPoints[ScrollStoppingPoints.Length - 1])
        {
            ScrollRightBtn.SetActive(false);
            ScrollCountText.text = ScrollStoppingPoints.Length.ToString();
        }
        else
        {
            ScrollRightBtn.SetActive(true);
        }
    }
    public void BackBtn()
    {
        for (int i = 0; i < InstentiatedChracters.childCount; i++)
        {
            Destroy(InstentiatedChracters.GetChild(i).gameObject);
        }
    }
    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void ClassicMode()
    {
        Mode = "Classic";
        PlayerPrefs.SetString("Mode", Mode);
    }
    public void AdvancedMode()
    {
        Mode = "Advanced Mode";
        PlayerPrefs.SetString("Mode", Mode);
    }
    public void SelectCity(string CityName)
    {
        City = CityName;
    }
    public void ChracterSelect()
    {
        if (PlayerTypeSelected)
        {
            CharactersBtn.SetActive(false);
            Chracter = NameText.text;
            Debug.Log("Character Name = " + Chracter);
            PlayerPrefs.SetString("Character", Chracter);
            switch (City.ToLower())
            {
                case "karachi":
                    SceneController.instance.LoadScene(Karachi);
                    break;
                case "lahore":
                    SceneController.instance.LoadScene(Lahore);
                    break;
                case "islamabad":
                    SceneController.instance.LoadScene(Islamabad);
                    break;
                case "village":
                    SceneController.instance.LoadScene(Village);
                    break;
                case "faislabad":
                    SceneController.instance.LoadScene(Faislabad);
                    break;
            }
        }
        else
        {
            WrongSelect = true;
            StartCoroutine(MessageBox("Please Select a player type", true));
        }
    }
    public void SrollNavigationLeft()
    {
        ScrollLeft = true;
        ScrollRight = false;
        CurruntPoint--;
        if(CurruntPoint < 0)
        {
            CurruntPoint = 0;
        }
        ScrollCountText.text = (CurruntPoint + 1).ToString();

    }
    public void SrollNavigationRight()
    {
        ScrollLeft = false;
        ScrollRight = true;
        CurruntPoint++;
        if(CurruntPoint > ScrollStoppingPoints.Length - 1)
        {
            CurruntPoint = ScrollStoppingPoints.Length - 1;
        }
        ScrollCountText.text = (CurruntPoint + 1).ToString();

    }
    public void CopyRightMusicOnOff(TextMeshProUGUI OnOffText)
    {
        if(OnOffText.text == "OFF")
        {
            OnOffText.text = "ON";
        }
        else
        {
            OnOffText.text = "OFF";
        }
    }
    public void AudioLanguageLeft()
    {
        if(CurruntLanguage - 1 >= 0)
        {
            LanguageText.text = AudioLanguage[CurruntLanguage - 1];
            CurruntLanguage--;
        }
        else
        {
            CurruntLanguage = AudioLanguage.Length - 1;
            LanguageText.text = AudioLanguage[CurruntLanguage];
        }
    }
    public void AudioLanguageRight()
    {
        if (CurruntLanguage + 1 <= AudioLanguage.Length - 1)
        {
            LanguageText.text = AudioLanguage[CurruntLanguage + 1];
            CurruntLanguage++;
        }
        else
        {
            CurruntLanguage = 0;
            LanguageText.text = AudioLanguage[CurruntLanguage];
        }
    }
    IEnumerator MessageBox(string Message , bool IsError)
    {
        if (IsError)
        {
            MessageTxt.gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            MessageTxt.gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        MessageTxt.gameObject.GetComponent<TextMeshProUGUI>().text = Message;
        MessageTxt.SetActive(true);
        yield return new WaitForSeconds(1f);
        WrongSelect = false;
        MessageTxt.SetActive(false);
    }
}