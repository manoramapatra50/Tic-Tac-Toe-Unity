using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager inst;
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);

    private GameObject board;

    [SerializeField]private GameObject boardPrefab;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI msgText;


    private void Awake()
    {
        if (inst != null && inst != this) Destroy(this.gameObject);
        else inst = this;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            Debug.Log(clientId + " joined");
            if(NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                SpawnBoard();
            }
        };
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("called");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void SpawnBoard()
    {
        board = Instantiate(boardPrefab);
        var instanceNetworkObject = board.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        GameManager.inst.currentTurn.Value = 0;
    }

    public void ShowMsg(string msg)
    {
        if(msg.Equals("won"))
        {
            msgText.text = "You Won";
            resultPanel.SetActive(true);
            ShowOpponentMsg("You Loose");
        }
        else if(msg.Equals("draw"))
        {
            msgText.text = "Game Draw";
            resultPanel.SetActive(true);
            ShowOpponentMsg("Game Draw");
        }
    }

    void ShowOpponentMsg(string msg)
    {
        if(IsHost)
        {
            ShowOpponentMsgClientRpc(msg);
        }
        else 
        {
            ShowOpponentMsgServerRpc(msg);
        }
    }
    [ClientRpc]
    void ShowOpponentMsgClientRpc(string msg)
    {
        if (IsHost) return;
        
        msgText.text = msg;
        resultPanel.SetActive(true);
    }
    [ServerRpc(RequireOwnership =false)]
    void ShowOpponentMsgServerRpc(string msg)
    {
        msgText.text = msg;
        resultPanel.SetActive(true);
    }

    public void Replay()
    {
        if(!IsHost)
        {
            ReplayServerRpc();
            resultPanel.SetActive(false);
        }
        else
        {
            Destroy(board);
            SpawnBoard();
            ReplayClientRpc();
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void ReplayServerRpc()
    {
        Destroy(board);
        SpawnBoard();
        resultPanel.SetActive(false);
    }

    [ClientRpc]
    public void ReplayClientRpc()
    {
        resultPanel.SetActive(false);
    }


}
