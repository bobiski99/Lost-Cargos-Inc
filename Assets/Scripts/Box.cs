using UnityEngine;

public class Box : MonoBehaviour
{
    public int color;
    public int number;
    public bool Danger = false;
    public bool cat = false;
    public bool hat = false;
    public bool seal = false;
    public bool discoball = false;
    public bool isOnTable = false;
    public bool CanBeScanned = false;


    void Start()
    {

        color = UnityEngine.Random.Range(1, 5);
        number = UnityEngine.Random.Range(1, 5);
        if (CompareTag("box"))
        {
            Danger = Random.value <= 0.05f;
        }
        if (tag == "catto")
        {
            cat= true;
            color = 1;
            number = 0;
        }

        if (tag == "discoball")
        {
            discoball = true;
        }

        if (tag == "hatto")
        {
            hat = true;
        }

        if (tag == "seal")
        {
            seal = true;
        }
    }
}