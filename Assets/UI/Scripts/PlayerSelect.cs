using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    #region PUblic Variables
    public float MaxScale;
    public float MinScale;
    public float ScalSpeed;
    public float BackGroundSpeed;
    public Image BackGround;
    [Space(10)]
    public string PlayerType;
    public UIManager uIManager;
    #endregion

    #region Private Variables
    bool PointerEnter;
    bool Selected;
    bool MoveWholeCircle;
    bool CanMoveCircle;
    bool CircleState;
    Color32 OrignalBackGroundColor;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CanMoveCircle = false;
        OrignalBackGroundColor = BackGround.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (uIManager.WrongSelect)
        {
            MoveWholeCircle = true;
            CanMoveCircle = true;
        }
        else
        {
            if (CanMoveCircle)
            {
                MoveWholeCircle = false;
            }
        }
        MoveCicle();
        if (!CircleState)
        {
            if (PointerEnter)
            {
                BackGround.fillClockwise = true;
                BackGround.color = OrignalBackGroundColor;
                if (transform.localScale.x < MaxScale && transform.localScale.y < MaxScale && transform.localScale.z < MaxScale)
                {
                    transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * ScalSpeed, transform.localScale.y + Time.deltaTime * ScalSpeed, transform.localScale.z + Time.deltaTime * ScalSpeed);
                }
                if (BackGround.fillAmount < 1)
                {
                    BackGround.fillAmount += Time.deltaTime * BackGroundSpeed;
                }
            }
            else
            {
                //BackGround.fillClockwise = true;
                if (transform.localScale.x > MinScale && transform.localScale.y > MinScale && transform.localScale.z > MinScale)
                {
                    transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * ScalSpeed, transform.localScale.y - Time.deltaTime * ScalSpeed, transform.localScale.z - Time.deltaTime * ScalSpeed);
                }
                if (BackGround.fillAmount > 0)
                {
                    BackGround.fillAmount -= Time.deltaTime * BackGroundSpeed;
                }
            }
        }
    }
    public void OnPointerEnter()
    {
        PointerEnter = true;
    }
    public void OnPointerExit()
    {
        if (!Selected && !uIManager.WrongSelect)
        {
            PointerEnter = false;
        }
    }
    public void OnSelected(string Type)
    {
        PlayerType = Type;
        PlayerPrefs.SetString("PlayerType", Type);
        Debug.Log("Player Type = " + Type);
        uIManager.PlayerTypeSelected = true;
        Selected = true;
        PointerEnter = true;
    }
    public void OnDeslected()
    {
        //uIManager.PlayerTypeSelected = false;
        Selected = false;
        PointerEnter = false;
    }
    void MoveCicle()
    {
        if (MoveWholeCircle)
        {
            BackGround.color = Color.red;
            CircleState = true;
            BackGround.fillClockwise = true;
            if (transform.localScale.x < MaxScale && transform.localScale.y < MaxScale && transform.localScale.z < MaxScale)
            {
                transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * ScalSpeed, transform.localScale.y + Time.deltaTime * ScalSpeed, transform.localScale.z + Time.deltaTime * ScalSpeed);
            }
            if (BackGround.fillAmount < 1)
            {
                BackGround.fillAmount += Time.deltaTime * BackGroundSpeed;
            }
        }
        else
        {
            if (CanMoveCircle)
            {
                BackGround.fillClockwise = false;
                if (transform.localScale.x > MinScale && transform.localScale.y > MinScale && transform.localScale.z > MinScale)
                {
                    transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime * ScalSpeed, transform.localScale.y - Time.deltaTime * ScalSpeed, transform.localScale.z - Time.deltaTime * ScalSpeed);
                }
                if (BackGround.fillAmount > 0)
                {
                    BackGround.fillAmount -= Time.deltaTime * BackGroundSpeed;
                }
                CanMoveCircle = false;
                CircleState = false;
            }
        }
    }
}
