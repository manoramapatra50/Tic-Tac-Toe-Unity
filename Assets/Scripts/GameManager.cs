using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager inst;
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);

    private GameObject board;

    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI msgText;

    [SerializeField] private TextMeshProUGUI joinCodeText;

    [SerializeField] private TMP_InputField joinCodeIP;

    [SerializeField] private TextMeshProUGUI[] turnTextMsg;

    public Timer timer;


    private void Awake()
    {
        if (inst != null && inst != this) Destroy(this.gameObject);
        else inst = this;
    }

    void Start()
    {
        timer.onTimeEnd += ChangeTurn;
    }

    public void SpawnBoard()
    {
        board = Instantiate(boardPrefab);
        var instanceNetworkObject = board.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        currentTurn.Value = 0;
        timer.StartTimer();
        StartTimerOnClientRpc();
    }
    [ClientRpc]
    public void StartTimerOnClientRpc()
    {
        timer.StartTimer();
    }

    public void ChangeTurn()
    {
        int v = currentTurn.Value;
        v++;
        if (v > 1) v = 0;
        currentTurn.Value = v;

        timer.StartTimer();
    }

    public void ShowMsg(string msg)
    {
        if (msg.Equals("won"))
        {
            msgText.text = "You Won";
            resultPanel.SetActive(true);
            ShowOpponentMsg("You Loose");
        }
        else if (msg.Equals("draw"))
        {
            msgText.text = "Game Draw";
            resultPanel.SetActive(true);
            ShowOpponentMsg("Game Draw");
        }
    }

    void ShowOpponentMsg(string msg)
    {
        if (IsHost)
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
    [ServerRpc(RequireOwnership = false)]
    void ShowOpponentMsgServerRpc(string msg)
    {
        msgText.text = msg;
        resultPanel.SetActive(true);
    }

    public void Replay()
    {
        if (!IsHost)
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
    [ServerRpc(RequireOwnership = false)]
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

    void OnDestroy()
    {
        timer.onTimeEnd -= ChangeTurn;
    }
}
