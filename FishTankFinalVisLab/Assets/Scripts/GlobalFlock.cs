using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject BigFishprefab;
    public GameObject Smallfishprefab;
    public static float tankSize = 21.21f;

    public static Vector3 goalPos = Vector3.zero;

    public static float TankFloorY = 0.5f;
    public static float TankTopY = 9.5f;
    
    static int numFish = 5;
    public static GameObject[] smallFish = new GameObject[numFish];
    public static GameObject[] bigFish = new GameObject[numFish];

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("SubMarineCamera");
        if (gos.Length > 0)
        {
            //Debug.Log(((Camera)gos[0].GetComponent("Camera")).backgroundColor);
            RenderSettings.fogColor = ((Camera)gos[0].GetComponent("Camera")).backgroundColor;
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fog = true;
        }
        else
        {
            RenderSettings.fogColor = new Color(0.271f, 0.427f, 0.863f, 0.000f);
            RenderSettings.fogDensity = 0.03f;
            RenderSettings.fog = true;
        }
        for (int i =0; i<numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize), Random.Range(TankFloorY, TankTopY), Random.Range(-tankSize, tankSize));
            smallFish[i] = (GameObject)Instantiate(Smallfishprefab, pos, Quaternion.identity);
        }
        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-tankSize, tankSize), Random.Range(TankFloorY, TankTopY), Random.Range(-tankSize, tankSize));
            bigFish[i] = (GameObject)Instantiate(BigFishprefab, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0,10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-tankSize, tankSize), Random.Range(TankFloorY, TankTopY), Random.Range(-tankSize, tankSize));
        }
    }

  }
