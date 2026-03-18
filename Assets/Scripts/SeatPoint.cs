using UnityEngine;
using TMPro;

public class SeatPoint : MonoBehaviour
{
    public bool isOccupied;
    public CustomerZone customerZone;
    public TextMeshProUGUI customerTextUI;
    public GameObject moneyObject;
    public GameObject servedSushiVisual;
    public GameObject servedRamenVisual;
    public GameObject servedRollVisual;
    public Transform[] enterWaypoints;
    public Transform[] exitWaypoints;
}
