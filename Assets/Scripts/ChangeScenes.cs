using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        int totalscenes = SceneManager.sceneCountInBuildSettings;
        int currentscene = SceneManager.GetActiveScene().buildIndex;
        int targetscene = currentscene + 1;
        if(targetscene >= totalscenes) targetscene = 0;
        
        SceneManager.LoadScene(targetscene);
    }
}
