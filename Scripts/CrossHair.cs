using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color doHighLightColor;
    Color orginalColor;
    private void Start()
    {
        Cursor.visible = false;
        orginalColor = dot.color;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * -100 * Time.deltaTime);
    }

    public void detectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = doHighLightColor;
        }
        else
        {
            dot.color = orginalColor;
        }
    }
}
