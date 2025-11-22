using UnityEngine;

public class LimitadorFrameRate : MonoBehaviour
{
    int limiteFrameRate = 30;
    private void Start()
    {
        Application.targetFrameRate = limiteFrameRate;
    }
}
