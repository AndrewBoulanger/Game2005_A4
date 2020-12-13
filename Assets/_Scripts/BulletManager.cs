using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public int maxBullets = 30;
    public Queue<GameObject> bulletPool;
    public Queue<GameObject> activeBullets;
    public GameObject BulletTemplate;
    // Start is called before the first frame update
    void Start()
    {
        bulletPool = new Queue<GameObject>();
        activeBullets = new Queue<GameObject>();
        PopulateBulletPool();

    }

    // Update is called once per frame
    void Update()
    {
        //return the oldest bullet if it is inactive
        if(activeBullets.Count > 0 && activeBullets.Peek().activeSelf == false)
            bulletPool.Enqueue(activeBullets.Dequeue());
    }


    private void PopulateBulletPool()
    {
        GameObject newBullet;

        for(int i = 0; i< maxBullets; i++)
        {
            newBullet = MonoBehaviour.Instantiate(BulletTemplate);
            newBullet.SetActive(false);
            bulletPool.Enqueue(newBullet);
        }
    }

    public GameObject getNextActiveBullet()
    {
        GameObject nextBullet;
        if(bulletPool.Peek() != null)
        {
            nextBullet = bulletPool.Dequeue();
        }
        else 
        {
           nextBullet = activeBullets.Dequeue();
        }

        nextBullet.GetComponent<BulletBehaviour>().Reset();
        activeBullets.Enqueue(nextBullet);
        return nextBullet;
    }
    
 

}
