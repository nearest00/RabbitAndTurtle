using UnityEngine;
using System;
public class MovingCarac : MonoBehaviour
{
    private float x = 0;
    private float CharacSpeed;
    public float SpeedSpeed = 2;
    // Update is called once per frame
    void FixedUpdate()
    {
        x=x+0.1f;
        CharacSpeed= SpeedSpeed*MathF.Sin(x);
        this.transform.Translate(0, CharacSpeed/100, 0);
    }
}
