using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable] //makes class show up in the inspector
    public class Pool //class with necessary attributes of each pool
    {
        public string tag; //what the tag will be
        public GameObject prefab; //what the pool will spawn
        public int size; //how many prefabs the pool will contain
    }

    #region Singleton
    //what is singleton?
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    //dictionary is like array but unordered and instead finds objects based on the key rather than a number
    //first is type of the keys and second is type of the objects stored
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public List<Pool> pools; //creating a list of pools
    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>(); //creating instance of dictionary

        foreach (Pool pool in pools) //kinda like declaring 2d arrays
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                //this creates one queue of gameobjects for one type of object
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + "doesnt exist");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); //poolDictionary[tag] gives us the correct type of object and dequeue takes out first element in queue

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);//reuse the object

        return objectToSpawn;
    }
}
