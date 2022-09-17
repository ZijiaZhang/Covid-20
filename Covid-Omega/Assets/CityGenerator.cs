using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public float Width = 5;
    public float XCount = 3;
    public float YCount = 3;
    
    public GameObject buildings;
    public void Start()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        for (int i = 0; i < XCount; i++)
        {
            for (int j = 0; j < YCount; j++)
            {
                GameObject obj = Instantiate(buildings,
                    gameObject.transform.position + new Vector3(i * Width,0, j * Width),
                    gameObject.transform.rotation);
                obj.transform.localScale = new Vector3(Width, Random.Range(10, 30), Width);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
