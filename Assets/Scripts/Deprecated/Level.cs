using System;
using System.Collections;
using System.Collections.Generic;
using PickMaster.Game.View;
using PickMaster.Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{

    public int levelDuration = 10;
    private float timer = 0.0f;

    public GameObject arrow;
    public GameObject redSector;


    public float nuggetValue;
    public float penaltyValue;
    public GameObject levelProgressBar;
    private Slider levelProgressSlider;
    private Image redSectorImage;
    private float levelProgress;

    public Material goldenMat;

    public GameObject coin;
    public GameObject ingot;
    public int coinsAmount;

    public GameObject goldenStuffInHand;
    public GameObject basicStuffInHand;
    public GameObject goldenPuffFX;

    public float goldenModeDuration;
    private ObjGenerator objGenerator;

    public TextMeshProUGUI coinsCollectedText;
    private int coinsCollected;

    private ConveyorBelt cb;

    private void Start()
    {
        objGenerator = FindObjectOfType<ObjGenerator>();
        cb = FindObjectOfType<ConveyorBelt>();
        levelProgress = 0.07f; //to compensate left bar part is beneath the clock icon. After first increase bar should be visible
        levelProgressSlider = levelProgressBar.GetComponent<Slider>();

        redSectorImage = redSector.GetComponent<Image>();

        coinsCollected = 0;

        UpdateUI();
        //coinsCollectedText.text = "123";
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < levelDuration)
        {
            float levelCompletion = timer / levelDuration;
            float arrowRotation = -360 * levelCompletion;
            arrow.transform.eulerAngles = new Vector3(0, 0, arrowRotation);
            //print((int)(levelCompletion * 100));
            //print(timer);

            redSectorImage.fillAmount = 1 - levelCompletion;

            var tempColor = redSectorImage.color;
            tempColor.a = levelCompletion;
            redSectorImage.color = tempColor;

        }
        else
        {
            print("Level Complete");
            EndLevel();
        }
    }

    void EndLevel()
    {
        SceneManager.LoadScene(SceneNames.MENU);
    }

    public void AddNugget()
    {
        if (levelProgress < 1)
        {
            levelProgress += nuggetValue;
            UpdateUI();
        }
        else
        {
            //ConvertAllItemsToGolden();
            GoldenMode();
        }

    }

    public void AddPenalty()
    {

        levelProgress -= penaltyValue;
        if (levelProgress < 0)
        {
            levelProgress = 0;
        }
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        UpdateUI();
    }


    void UpdateUI()
    {
        levelProgressSlider.value = levelProgress;
        
        //making this fuking american style nnumbers 1,123
        if (coinsCollected >= 1000)
        {
            int thousands = (int)(coinsCollected / 1000);
            int lessThenThousand = coinsCollected - thousands * 1000;
            string rightPart = "";
            if (lessThenThousand >=100)
            {
                rightPart = lessThenThousand.ToString();
                
            } else if (lessThenThousand >= 10)
            {
                rightPart = "0" + lessThenThousand.ToString();
            } else
            {
                rightPart = "00" + lessThenThousand.ToString();
            }
            coinsCollectedText.text = thousands.ToString() + "," + rightPart;
        } else 
            coinsCollectedText.text = coinsCollected.ToString();
    }

    void ConvertAllItemsToGolden()
    {
        print("Converting to golden!");
        List<GameObject> allItems = new List<GameObject>();
        allItems.AddRange(GameObject.FindGameObjectsWithTag("Candy"));
        allItems.AddRange(GameObject.FindGameObjectsWithTag("Cookie"));

        //List<Material> allMats = new List<Material>();

        for (int i = 0; i < allItems.Count; i++)
        {
            print(allItems[i].name);
            foreach (Transform child in allItems[i].transform)
            {
                //print("child item found: " + child.gameObject.name);
                Material[] mat = child.gameObject.GetComponent<MeshRenderer>().materials;
                for (int j = 0; j < mat.Length; j++)
                {
                    print(mat[j]);
                    mat[j] = goldenMat;
                }
                child.gameObject.GetComponent<MeshRenderer>().materials = mat;
            };
        }
    }

    void ConvertAllToCoins()
    {
        List<GameObject> allItems = new List<GameObject>(); // take all items
        allItems.AddRange(GameObject.FindGameObjectsWithTag("Candy"));
        allItems.AddRange(GameObject.FindGameObjectsWithTag("Cookie"));



        for (int i = 0; i < allItems.Count; i++)
        {
            GameObject newIngot = Instantiate(ingot, allItems[i].transform.position, UnityEngine.Random.rotation); //make 1 ingot
            Destroy(newIngot, 15);
            for (int j = 0; j < coinsAmount; j++)
            {
                GameObject newCoin = Instantiate(coin, allItems[i].transform.position, UnityEngine.Random.rotation); //make many coins (coinsAmount var)
                Destroy(newCoin, 15);
            }

            Destroy(allItems[i]);
        }
    }

    void SwapHandItemsToGolden()
    {
        goldenStuffInHand.SetActive(true);
        basicStuffInHand.SetActive(false);
        GameObject fx = Instantiate(goldenPuffFX, goldenStuffInHand.transform.position, Quaternion.identity);
        Destroy(fx, 2);
    }

    void SwapHandItemsToNormal()
    {
        goldenStuffInHand.SetActive(false);
        basicStuffInHand.SetActive(true);
        GameObject fx = Instantiate(goldenPuffFX, goldenStuffInHand.transform.position, Quaternion.identity);
        Destroy(fx, 2);
    }

    private void GoldenMode()
    {
        ConvertAllToCoins();
        SwapHandItemsToGolden();
        objGenerator.EnableGoldenMode();
        //cb.resetSpeed(); // reset speed when in golden mode;
        //yield return new WaitForSeconds(goldenModeDuration);
        StartCoroutine(GoldenModeProgress());
    }

    private IEnumerator GoldenModeProgress()
    {
        float goldenTimer = goldenModeDuration;
        while (goldenTimer > 0)
        {
            goldenTimer -= Time.deltaTime;
            levelProgress = (1 / goldenModeDuration) * goldenTimer;
            print(levelProgress);
            UpdateUI();
            yield return new WaitForEndOfFrame();
            
        }
        print("Golden mode complete!");
        levelProgress = 0.07f; //start value
        objGenerator.DisableGoldenMode();
        SwapHandItemsToNormal();
    }

}
