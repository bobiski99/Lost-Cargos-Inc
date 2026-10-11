using UnityEngine;
using UnityEngine.SceneManagement; 
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private ContractFPSController_sc controller_off;
    [Header("Scene Settings")]
    public string sceneToLoad;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadTargetScene();

            controller_off.controllerEnabled = false;
        }
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
