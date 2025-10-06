using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CurrentDateTimeText : MonoBehaviour
{

    TMP_Text dateText;
    // Start is called before the first frame update
    void Start()
    {
        dateText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime dt = DateTime.Now;
        dateText.text = dt.ToString();
    }
}
