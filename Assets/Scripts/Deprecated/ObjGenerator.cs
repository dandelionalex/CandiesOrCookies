using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGenerator : MonoBehaviour
{

    public float generationRate = 0.2f;
    public List<GameObject> objects;
    public GameObject objectsGenerator;

    private bool golden = false;

    public GameObject ingot;
    public GameObject coin;
    public int coinsAmount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenObjects());
    }

    IEnumerator GenObjects()
    {
        while (!golden)
        {
            print("generating NORMAL");
            GameObject newObj = Instantiate(objects[Random.Range(0, objects.Count)], objectsGenerator.transform.position, Random.rotation);
            //Destroy(newObj, 10);
            yield return new WaitForSeconds(generationRate);
        }

        while (golden) //TODO: DRY violated - in level class just the same.
        {
            print("generating GOLDEN");
            GameObject newIngot = Instantiate(ingot, objectsGenerator.transform.position, Random.rotation);
            Destroy(newIngot, 15);

            for (int j = 0; j < coinsAmount; j++)
            {
                GameObject newCoin = Instantiate(coin, objectsGenerator.transform.position, Random.rotation); //make many coins (coinsAmount var)
                Destroy(newCoin, 15);
            }

            yield return new WaitForSeconds(generationRate);
        }
    }

    public void EnableGoldenMode()
    {
        golden = true;
    }

    public void DisableGoldenMode()
    {
        golden = false;
        StartCoroutine(GenObjects());
    }

}
