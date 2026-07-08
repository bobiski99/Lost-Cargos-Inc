using UnityEngine;

public class Box : MonoBehaviour
{
    public int color;
    public int number;
    public bool Danger = false;
    public bool cat = false;
    public bool hat = false;
    public bool seal = false;
    private float danger_percentage = 0.5f; //%5 ihtimal


    void Start()
    {

        Danger = UnityEngine.Random.value <= danger_percentage;
        color = UnityEngine.Random.Range(1, 5);
        number = UnityEngine.Random.Range(1, 5);
        if (tag == "catto")
        {
            cat= true;
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