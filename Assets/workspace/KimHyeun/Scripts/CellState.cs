using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellState : MonoBehaviour
{
    public GameObject dataObj;
    public Image cell_Image;
    public TMP_Text nameText;
    public TMP_Text subText1;
    public TMP_Text subText2;
    public TMP_Text subText3;
    public CellType cellType;

    public GameObject buttonObj;

    public enum CellType
    {
        None,
        Shop,
        Record,
        Ranking
    }
}
