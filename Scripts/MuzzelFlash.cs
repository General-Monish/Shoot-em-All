using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzelFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public SpriteRenderer[] SpriteRenderers;

    public float flashTime;
    // Start is called before the first frame update
    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);
      
        Invoke("Deactivate", flashTime);
    }

    void Deactivate()
    {
        flashHolder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
