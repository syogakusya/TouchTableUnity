using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemPool : MonoBehaviour
{
    private IObjectPool<PooledItem> objectPool;

    [SerializeField] private bool collectionCheck = true;
    [SerializeField] private int defultCpacity = 20;
    [SerializeField] private int maxSize = 100;
    [SerializeField] PooledItem pooledItemPrefab;
    [SerializeField] private bool antiGravity = false;
    [SerializeField] private float emitterRange = 9.0f;

    private void Awake()
    {
        objectPool = new ObjectPool<PooledItem>(
            CreateItem,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPooledObject,
            collectionCheck,
            defultCpacity,
            maxSize);
    }

    private PooledItem CreateItem()
    {
        PooledItem item = Instantiate(pooledItemPrefab, 
            new Vector3 (Random.Range(-emitterRange,emitterRange) + transform.position.x,transform.position.y,transform.position.z - 1.0f),
            Quaternion.identity);
        item.ObjectPool = objectPool;
        item.Gravity = antiGravity;
        return item;
    }

    private void OnGetFromPool(PooledItem pooledObject)
    {
        pooledObject.gameObject.transform.position = new Vector3(Random.Range(-9.0f, 9.0f) + transform.position.x, transform.position.y, transform.position.z - 1.0f);
        pooledObject.gameObject.GetComponent<Rigidbody>().velocity= Vector3.zero;
        pooledObject.Gravity = antiGravity;
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(PooledItem pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(PooledItem pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }

    private void Update()
    {
        objectPool.Get();
    }
}
