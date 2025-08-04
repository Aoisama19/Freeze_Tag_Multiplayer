using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sliders : MonoBehaviour
{
    public TextMeshProUGUI PercentageText;
    public bool Percentage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Percentage)
        {
            PercentageText.text = transform.gameObject.GetComponent<Slider>().value.ToString("f0") + "%";
        }
        else
        {
            PercentageText.text = transform.gameObject.GetComponent<Slider>().value.ToString("f1");
        }
    }
    public void Left()
    {
        transform.gameObject.GetComponent<Slider>().value--;
    }
    public void Right()
    {
        transform.gameObject.GetComponent<Slider>().value++;
    }

}
