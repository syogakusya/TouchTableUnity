using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledItem : MonoBehaviour
{
    private IObjectPool<PooledItem> objectPool;

    public IObjectPool<PooledItem> ObjectPool { set => objectPool = value; }

    private Rigidbody rigidBody;
    private bool antiGravity = false;
    public bool Gravity { set => antiGravity = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ObjectPoolReleaser"))
        {
            objectPool.Release(this);
        }
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(antiGravity)
        {
            rigidBody.useGravity = false;
            rigidBody.AddForce(new Vector3(0, 9.8f, 0), ForceMode.Acceleration);
        }
        else
        {
            rigidBody.useGravity=true;
        }
    }
}
