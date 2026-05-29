using UnityEngine;

namespace SGame.UI
{
    public class StorePanelController : MonoBehaviour
    {
        [SerializeField] private CoinBalanceLabel coinBalanceLabel;
        [SerializeField] private StoreItemController[] itemControllers;

        private void OnEnable()
        {
            RefreshAll();
        }

        public void RefreshAll()
        {
            coinBalanceLabel?.Refresh();
            if (itemControllers == null)
            {
                return;
            }

            foreach (var item in itemControllers)
            {
                item?.Refresh();
            }
        }
    }
}
