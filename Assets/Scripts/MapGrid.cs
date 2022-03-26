using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Column {A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,y,Z}
public class MapGrid : MonoBehaviour
{
    public Selector Selector;
    public GameObject ButtonPrefab;
    public GameObject RowPrefab;
    public string Valid;
    public int ColumnCount;
    int RowCount => Mathf.RoundToInt(ColumnCount * 0.75f);

    void Awake()
    {
        if (!ButtonPrefab || !RowPrefab) return;

        transform.ClearChild(false);

        
        for (int row = 0; row < RowCount; row++)
        {
            GameObject _row = Instantiate(RowPrefab, transform);
            _row.name = "Row " + row;
            for (int col = 0; col < ColumnCount; col++)
            {
                GameObject column = Instantiate(ButtonPrefab, _row.transform);
                column.name = "Row " + row + " Col " + (Column)col;
                column.GetComponent<GridButton>().Selector = Selector;

                if (Valid == column.name)
                {
                    column.GetComponent<GridButton>().IsValid = true;
                    if(GameManager.Instance.Debug)
                    {
                        ColorBlock color = ColorBlock.defaultColorBlock;
                        color.normalColor = Color.red;
                        column.GetComponent<Button>().colors = color;
                    }
                   
                    
                }
            }
        }
        
    }
    void OnValidate()
    {
        //Debug.Log("Column: " + ColumnCount + " Row: " + RowCount);   
    }
}
