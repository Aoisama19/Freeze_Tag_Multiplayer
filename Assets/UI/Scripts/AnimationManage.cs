using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationManage : MonoBehaviour
{
    #region Public Variables
    public float MaxScale;
    public float MinScale;
    public float ScaleSpeed;
    [Space(10)]
    public bool UpdateText;
    public TextMeshProUGUI Text;
    public string Subject;
    [Space(10)]
    public bool BackGroundEffect;
    public float BackGroundSpeed;
    public BackGroundEffect BackGround;
    [Space(10)]
    public bool ChracterSelect;
    public GameObject Chracter;
    public Transform ChracterPoint;
    public Transform InstentiatedChracters;
    [Space(10)]
    public bool ChangeColor;
    public Color32 TextColor;
    public Color32 ImageColor;
    public TextMeshProUGUI TextToChangeColor;
    public Image ImageToChangeColor;


    #endregion

    #region Private Variables
    bool IsPointerEntered;
    GameObject ChracterToinstentiate;
    Color32 OrignalTextColor;
    Color32 OrignalImageColor;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        IsPointerEntered = false;
        if (ChangeColor)
        {
            if (TextToChangeColor != null)
            {
                OrignalTextColor = TextToChangeColor.color;
            }
            if (ImageToChangeColor != null)
            {
                OrignalImageColor = ImageToChangeColor.color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPointerEntered)
        {
            if(transform.localScale.x < MaxScale && transform.localScale.y < MaxScale && transform.localScale.z < MaxScale)
            {
                transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * ScaleSpeed, transform.localScale.y + Time.deltaTime * ScaleSpeed , transform.localScale.z + Time.deltaTime * ScaleSpeed);
            }
            if (BackGroundEffect)
            {
                if(BackGround.fillAmount < 1)
                {
                    BackGround.fillAmount += Time.deltaTime * BackGroundSpeed;
                }
            }
        }
        else
        {
            if (transform.localScale.x > MinScale && transform.localScale.y > MinScale && transform.localScale.z > MinScale)
            {
                transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * ScaleSpeed, transform.localScale.y - Time.deltaTime * ScaleSpeed, transform.localScale.z - Time.deltaTime * ScaleSpeed);
            }
            if (BackGround != null && BackGround.fillAmount > 0)
            {
                BackGround.fillAmount -= Time.deltaTime * BackGroundSpeed;
            }
        }
    }

    public void OnPointerEnter()
    {
        IsPointerEntered = true;
        if (UpdateText)
        {
            Text.gameObject.SetActive(true);
            if (UpdateText)
            {
                if (Text != null)
                {
                    Text.text = Subject;
                }
            }
        }    
        if (ChracterSelect)
        {
            if(Chracter != null)
            {
                for (int i = 0; i < InstentiatedChracters.childCount; i++)
                {
                    Destroy(InstentiatedChracters.GetChild(i).gameObject);
                }
                ChracterToinstentiate = Instantiate(Chracter, InstentiatedChracters);
                ChracterToinstentiate.transform.position = ChracterPoint.position;
                ChracterToinstentiate.transform.rotation = ChracterPoint.rotation;
            }
        }
        if (ChangeColor)
        {
            if (TextToChangeColor != null)
            {
                TextToChangeColor.color = TextColor;
            }
            if (ImageToChangeColor != null)
            {
                ImageToChangeColor.color = ImageColor;
            }
        }
    }
    public void OnPointerExit()
    {
        IsPointerEntered = false;
        if (UpdateText)
        {
            Text.gameObject.SetActive(false);
        }
        if (ChangeColor)
        {
            if (TextToChangeColor != null)
            {
                TextToChangeColor.color = OrignalTextColor;
            }
            if (ImageToChangeColor != null)
            {
                ImageToChangeColor.color = OrignalImageColor;
            }
        }
    }
    public void OnClick()
    {
        IsPointerEntered = false;
    }

}
