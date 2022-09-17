using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMultipleBlock : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float XCount = 3;
    public float YCount = 3;
    public GameObject blocks;
    void Start()
    {
        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                GameObject obj = Instantiate(blocks,
                    gameObject.transform.position + new Vector3(i * 30,0, j * 30),
                    gameObject.transform.rotation);
                // obj.transform.localScale = new Vector3(Width, Random.Range(10, 30), Width);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
