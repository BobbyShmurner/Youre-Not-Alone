using UnityEngine;

[CreateAssetMenu(fileName = "MazePieces", menuName = "ScriptableObjects/MazePieces")]
public class MazePieces : ScriptableObject
{
    public GameObject End;
    public GameObject Straight;
    public GameObject Corner;
    public GameObject TSection;
    public GameObject Cross;
}