using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BoardManager : NetworkBehaviour
{
    Button[,] allBtn = new Button[3, 3];
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
        if(NetworkManager.Singleton.IsHost)
        {
            allBtn[row, col].GetComponent<Image>().sprite = xSprite;
            ChangeSpriteOnClientRPC(row, col);

            Debug.Log("is host");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            allBtn[row, col].GetComponent<Image>().sprite = oSprite;
            ChangeSpriteOnServerRPC(row, col);

            Debug.Log("is client");
        }
    }

    [ClientRpc]
    void ChangeSpriteOnClientRPC(int row,int col)
    {
        allBtn[row, col].GetComponent<Image>().sprite = xSprite;
        allBtn[row, col].interactable = false;

        Debug.Log("is client server");
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeSpriteOnServerRPC(int row, int col)
    {
        allBtn[row, col].GetComponent<Image>().sprite = oSprite;
        allBtn[row, col].interactable = false;

        Debug.Log("is host server");
    }
}
