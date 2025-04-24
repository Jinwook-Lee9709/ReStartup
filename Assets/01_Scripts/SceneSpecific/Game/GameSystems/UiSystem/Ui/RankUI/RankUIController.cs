using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUIController : MonoBehaviour
{
    [SerializeField] private Button localRankingButton;
    [SerializeField] private Button globalRankingButton;
    [SerializeField] private GameObject localRankingPanel;
    [SerializeField] private GameObject globalRankingPanel;
    [SerializeField] private PlayerClone localPlayerClone;
    
    // Start is called before the first frame update
    void Start()
    {
        localRankingButton.onClick.RemoveListener(OnLocalRankingButtonTouched);
        globalRankingButton.onClick.RemoveListener(OnLocalRankingButtonTouched);
        localRankingButton.onClick.AddListener(OnLocalRankingButtonTouched);
        globalRankingButton.onClick.AddListener(OnGlobalRankingButtonTouched);
    }
    
    private void OnLocalRankingButtonTouched()
    {
        localRankingPanel.SetActive(true);
        globalRankingPanel.SetActive(false);
    }
    
    private void OnGlobalRankingButtonTouched()
    {
        localRankingPanel.SetActive(false);
        globalRankingPanel.SetActive(true);
        localPlayerClone.OnUnActive();
    }
    
    
    
}
