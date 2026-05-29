using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Attach to a Button. Reloads the active scene on click. Used by the
    /// "Play Again" button on the win overlay.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ReloadSceneOnClick : MonoBehaviour
    {
        [SerializeField] private Button button;

        public void Bind(Button btn)
        {
            button = btn;
        }

        private void OnEnable()
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(Reload);
                button.onClick.AddListener(Reload);
            }
        }

        private void OnDisable()
        {
            if (button != null) button.onClick.RemoveListener(Reload);
        }

        private void Reload()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
