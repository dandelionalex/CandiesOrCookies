using Facebook.Unity;
using UnityEngine;

public class FacebookIntegration : MonoBehaviour
{
    private void Awake()
    {
        FB.Init(OnInitComplete, this.OnHideUnity);
    }

    private void OnInitComplete()
    {
        Debug.Log("OnFBInitComplete");
    }

    private void OnHideUnity(bool isGameShown)
    {
        
    }
}
