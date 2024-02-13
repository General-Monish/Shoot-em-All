using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody rb;
    public float forceMin;
    public float forceMax;

    float lifeTime=4;
    float FadeTime=2;
    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);
    }

    IEnumerator fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / FadeTime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
  
}
