using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {

    // Método que cambia de escena al recibir el nombre de la escena
    public void ChangeScene(string sceneName) { SceneManager.LoadScene(sceneName); }

}
