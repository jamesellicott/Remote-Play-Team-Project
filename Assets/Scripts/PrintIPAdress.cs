using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PrintIPAdress : MonoBehaviour
{
    string IPAddress;

    public TextMeshProUGUI IPText;

    // Start is called before the first frame update
    void Start()
    {
        //retrives the name of this computer
        string computerName = Dns.GetHostName();

        for(int i = 0; i <= Dns.GetHostEntry(computerName).AddressList.Length - 1; i++)
        {
            if(Dns.GetHostEntry(computerName).AddressList[i].IsIPv6LinkLocal == false)
            {
                IPAddress = Dns.GetHostEntry(computerName).AddressList[i].ToString();
            }
        }

        Debug.Log(IPAddress);
        IPText.text = IPAddress;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
