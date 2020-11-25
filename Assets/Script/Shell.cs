using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody rb;
    public float minForce, maxForce;

    public float lifetime = 4.0f;
    public float fadetime = 1.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        float force = Random.Range(minForce, maxForce);
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {   // 等待lifetime后再消失
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<MeshRenderer>().material;
        Color initialColor = mat.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;// 下一帧再执行后续代码
        }

        Destroy(gameObject);
    }

}
