using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private void Awake()
    {
        ObjectPools.Clear();
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector2 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = ObjectPools.Find(p=>p.LookupString == objectToSpawn.name);
        if (pool == null)
        {
            var container = new GameObject(objectToSpawn.name);
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name, ParentGo = container };
            ObjectPools.Add(pool);
        }
        
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation, pool.ParentGo.transform);
            spawnableObj.name = objectToSpawn.name;
        }
        else
        { 
            spawnableObj.name = objectToSpawn.name;
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectPool(GameObject obj)
    {
        string goName = obj.name;

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);
        if (pool == null)
        {
            Debug.LogWarning($"{obj.name} No pool to return this obj");
            obj.SetActive(false);
        }
        else
        { 
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public GameObject ParentGo;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}