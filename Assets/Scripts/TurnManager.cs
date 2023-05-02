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

    private GameObject startGameBtn;

    private void Start()
    {
        startGameBtn = GameObject.Find("StartGameBtn");
    }

    public void StartGame()
    {
        CmdStartGame();
    }

    [Command(requiresAuthority = false)]
    private void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        GameObject[] clients = GameObject.FindGameObjectsWithTag("Player");

        if (clients[0].GetComponent<NetworkIdentity>().isServer)
        {
            if (clients[0].GetComponent<NetworkIdentity>().isLocalPlayer)
                hostID = clients[0].GetComponent<NetworkIdentity>();
            else
                clientID = clients[0].GetComponent<NetworkIdentity>();

            if (clients[1].GetComponent<NetworkIdentity>().isLocalPlayer)
                hostID = clients[1].GetComponent<NetworkIdentity>();
            else
                clientID = clients[1].GetComponent<NetworkIdentity>();
        }
        else
        {
            if (!clients[0].GetComponent<NetworkIdentity>().isLocalPlayer)
                hostID = clients[0].GetComponent<NetworkIdentity>();
            else
                clientID = clients[0].GetComponent<NetworkIdentity>();

            if (!clients[1].GetComponent<NetworkIdentity>().isLocalPlayer)
                hostID = clients[1].GetComponent<NetworkIdentity>();
            else
                clientID = clients[1].GetComponent<NetworkIdentity>();
        }

        hostID.GetComponent<PlayerManager>().DrawCards(5);
        clientID.GetComponent<PlayerManager>().DrawCards(5);

        startGameBtn.SetActive(false);
        if(!isServer) { clientID.GetComponent<PlayerManager>().TurnOffEndTurnBtn(); }
    }

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

            if (clients[0].GetComponent<NetworkIdentity>().isServer)
            {

                if (clients[0].GetComponent<NetworkIdentity>().isLocalPlayer)
                    hostID = clients[0].GetComponent<NetworkIdentity>();
                else
                    clientID = clients[0].GetComponent<NetworkIdentity>();

                if (clients[1].GetComponent<NetworkIdentity>().isLocalPlayer)
                    hostID = clients[1].GetComponent<NetworkIdentity>();
                else
                    clientID = clients[1].GetComponent<NetworkIdentity>();
            }
            else
            {
                if (!clients[0].GetComponent<NetworkIdentity>().isLocalPlayer)
                    hostID = clients[0].GetComponent<NetworkIdentity>();
                else
                    clientID = clients[0].GetComponent<NetworkIdentity>();

                if (!clients[1].GetComponent<NetworkIdentity>().isLocalPlayer)
                    hostID = clients[1].GetComponent<NetworkIdentity>();
                else
                    clientID = clients[1].GetComponent<NetworkIdentity>();
            }
        }
        else
        {
            hostsTurn = !hostsTurn;
            if (hostsTurn)
            {
                hostID.GetComponent<PlayerManager>().DrawCards(1);

                hostID.GetComponent<PlayerManager>().mana++;
                if (hostID.GetComponent<PlayerManager>().mana > 10)
                    hostID.GetComponent<PlayerManager>().mana = 10;

                hostID.GetComponent<PlayerManager>().CmdUpdateManaBar();

                if (clientID.GetComponent<PlayerManager>().attackingCard != null)
                {
                    clientID.GetComponent<PlayerManager>().attackingCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    clientID.GetComponent<PlayerManager>().attackingCard = null;
                }

                if (isServer)
                    hostID.GetComponent<PlayerManager>().TurnOnEndTurnBtn();
                else
                    clientID.GetComponent<PlayerManager>().TurnOffEndTurnBtn();
            }
            else
            {
                clientID.GetComponent<PlayerManager>().DrawCards(1);

                clientID.GetComponent<PlayerManager>().mana++;
                if (clientID.GetComponent<PlayerManager>().mana > 10)
                    clientID.GetComponent<PlayerManager>().mana = 10;

                clientID.GetComponent<PlayerManager>().CmdUpdateManaBar();

                if (hostID.GetComponent<PlayerManager>().attackingCard != null)
                {
                    hostID.GetComponent<PlayerManager>().attackingCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    hostID.GetComponent<PlayerManager>().attackingCard = null;
                }

                if (!isServer)
                    clientID.GetComponent<PlayerManager>().TurnOnEndTurnBtn();
                else
                    hostID.GetComponent<PlayerManager>().TurnOffEndTurnBtn();
            }
        }
    }

    private void Update()
    {
        Debug.Log("Hosts Turn : " + hostsTurn);
    }
}
