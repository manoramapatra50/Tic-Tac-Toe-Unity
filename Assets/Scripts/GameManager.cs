using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    [SerializeField]private GameObject boardPrefab;

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
        GameObject board = Instantiate(boardPrefab);
        var instanceNetworkObject = board.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}
