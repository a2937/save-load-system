using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private string NextSceneName;

    [SerializeField]
    private Vector3 newPlayerCoordinates;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(NextSceneName.Trim() != "")
            {
                collision.gameObject.transform.position = newPlayerCoordinates;
                DataPersistenceManager.instance.ReadSceneData();
                SceneManager.LoadScene(NextSceneName); 
            }
            else
            {
                Debug.LogWarning("Next scene name is null on Portal. Will not able to load next scene");
            }
        }
    }
}
