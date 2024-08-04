using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BoardManager : NetworkBehaviour
{
    Button[,] allBtn = new Button[3, 3];
    int[,] allBtnValue = new int[3, 3];
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    public override void OnNetworkSpawn()
    {
        var btns = GetComponentsInChildren<Button>();
        int k = 0;
        for(int i = 0; i< 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                allBtn[i, j] = btns[k];
                allBtnValue[i, j] = -1;
                k++;

                int row = i;
                int col = j;

                allBtn[i, j].onClick.AddListener(delegate
                {
                    OnClickBtn(row, col);
                });
            }
        }
    }

    void OnClickBtn(int row, int col)
    {
        if(NetworkManager.Singleton.IsHost && GameManager.inst.currentTurn.Value == 0)
        {
            allBtnValue[row, col] = 1;
            allBtn[row, col].GetComponent<Image>().sprite = xSprite;
            ChangeSpriteOnClientRPC(row, col);
            GameManager.inst.currentTurn.Value = 1;
            CheckResult(row, col);
            Debug.Log("is host");
        }
        else if (!NetworkManager.Singleton.IsHost && GameManager.inst.currentTurn.Value == 1)
        {
            allBtnValue[row, col] = 0;
            allBtn[row, col].GetComponent<Image>().sprite = oSprite;
            ChangeSpriteOnServerRPC(row, col);
            CheckResult(row, col);
            Debug.Log("is client");
        }
    }

    [ClientRpc]
    void ChangeSpriteOnClientRPC(int row,int col)
    {
        allBtnValue[row, col] = 1;
        allBtn[row, col].GetComponent<Image>().sprite = xSprite;
        allBtn[row, col].interactable = false;

        Debug.Log("is client server");
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeSpriteOnServerRPC(int row, int col)
    {
        allBtnValue[row, col] = 0;
        allBtn[row, col].GetComponent<Image>().sprite = oSprite;
        allBtn[row, col].interactable = false;

        GameManager.inst.currentTurn.Value = 0;

        Debug.Log("is host server");
    }

    void CheckResult(int row, int col)
    {
       
        if(IsWon(row, col))
        {
            GameManager.inst.ShowMsg("won");
        }
        else
        {
            if (IsGameDraw())
            {
                GameManager.inst.ShowMsg("draw");
            }
        }
    }

    public bool IsWon(int row, int col)
    {
        int clickedBtnValue = allBtnValue[row, col];
        // check for all column
        if (allBtnValue[0, col] == clickedBtnValue && allBtnValue[1, col] == clickedBtnValue && allBtnValue[2, col] == clickedBtnValue) return true;

        // check for all row
        if (allBtnValue[row, 0] == clickedBtnValue && allBtnValue[row, 1] == clickedBtnValue && allBtnValue[row, 2] == clickedBtnValue) return true;

        // check for diagonals
        if (allBtnValue[0, 0] == clickedBtnValue && allBtnValue[1, 1] == clickedBtnValue && allBtnValue[2, 2] == clickedBtnValue) return true;

        if (allBtnValue[0, 2] == clickedBtnValue && allBtnValue[1, 1] == clickedBtnValue && allBtnValue[2, 0] == clickedBtnValue) return true;

        return false;
    }

    bool IsGameDraw()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (allBtnValue[i, j] == -1) return false;
            }
        }
        return true;
    }
}
