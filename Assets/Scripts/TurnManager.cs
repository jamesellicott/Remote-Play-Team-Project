using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TurnManager : NetworkBehaviour
{
    public bool hostsTurn = true;

    public NetworkIdentity hostID;
    public NetworkIdentity clientID;

    public GameObject[] clients;

    public void EndTurn()
    {
        Debug.Log("End Turn Btn Pressed");
        if (clients.Length == 0)
        {
            Debug.Log("Find Clients");
            clients = GameObject.FindGameObjectsWithTag("Player");

            Debug.Log(clients);

            if (clients[0].GetComponent<NetworkIdentity>().isServer)
                hostID = clients[0].GetComponent<NetworkIdentity>();
            else
                clientID = clients[0].GetComponent<NetworkIdentity>();

            if (clients[1].GetComponent<NetworkIdentity>().isServer)
                hostID = clients[1].GetComponent<NetworkIdentity>();
            else
                clientID = clients[1].GetComponent<NetworkIdentity>();
        }
        else
        {
            CmdEndTurn();
        }
    }

    [Command]
    private void CmdEndTurn()
    {
        RpcEndTurn();
    }

    [ClientRpc]
    private void RpcEndTurn()
    {
        hostsTurn = !hostsTurn;
        if (hostsTurn)
        {
            hostID.GetComponent<PlayerManager>().DrawCards(1);
            hostID.GetComponent<PlayerManager>().TurnOnEndTurnBtn();
            clientID.GetComponent<PlayerManager>().TurnOffEndTurnBtn();
        }
        else
        {
            clientID.GetComponent<PlayerManager>().DrawCards(1);
            clientID.GetComponent<PlayerManager>().TurnOnEndTurnBtn();
            hostID.GetComponent<PlayerManager>().TurnOffEndTurnBtn();
        }
    }

    private void Update()
    {
        Debug.Log("Hosts Turn : " + hostsTurn);
    }
}
