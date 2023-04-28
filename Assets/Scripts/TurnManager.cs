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

    public void EndTurn()
    {
        Debug.Log("Local");
        CmdEndTurn();
    }

    [Command(requiresAuthority = false)]
    private void CmdEndTurn()
    {
        Debug.Log("Cmd");
        RpcEndTurn();
    }

    [ClientRpc]
    private void RpcEndTurn()
    {
        Debug.Log("Rpc");
        if (hostID == null || clientID == null)
        {
            GameObject[] clients = GameObject.FindGameObjectsWithTag("Player");

            if (clients[0].GetComponent<NetworkIdentity>().isLocalPlayer && clients[0].GetComponent<NetworkIdentity>().isServer)
                hostID = clients[0].GetComponent<NetworkIdentity>();
            else
                clientID = clients[0].GetComponent<NetworkIdentity>();

            if (clients[1].GetComponent<NetworkIdentity>().isLocalPlayer && clients[1].GetComponent<NetworkIdentity>().isServer)
                hostID = clients[1].GetComponent<NetworkIdentity>();
            else
                clientID = clients[1].GetComponent<NetworkIdentity>();
        }
        else
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
    }

    private void Update()
    {
        Debug.Log("Hosts Turn : " + hostsTurn);
    }
}
