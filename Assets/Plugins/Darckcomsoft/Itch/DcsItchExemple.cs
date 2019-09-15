using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using darckcomsoft.itch;
using System.Text;

public class DcsItchExemple : MonoBehaviour
{
    public ItchAPi Itch;

    void Start()
    {
        Itch = gameObject.AddComponent<ItchAPi>();

        if (Application.isEditor)
        {
#if UNITY_EDITOR
            Itch.EditorApiKey = "5av3kO2VL0iQuBu3zp7TNTbb6b257OYL81R3KQQ6";
            Itch.ServerApiKey = "5av3kO2VL0iQuBu3zp7TNTbb6b257OYL81R3KQQ6";
            Itch.StartItchApi(true, "122257");

            Debug.Log("Have Donwload Key: " + Itch.HaveDownloadKey("870669"));
            Debug.Log("Have Purchased: " + Itch.UserPurchase("870669", false));
            Debug.Log("Is A DONATOR: " + Itch.UserPurchase("870669", true));
#endif
        }
        else
        {
            Itch.StartItchApi(false);

            Debug.Log("UserId : " + Itch.GetMyUserId());
            Debug.Log("UserName : " + Itch.GetMyUserName());
            Debug.Log("UserImage : " + Itch.GetMyUserImg().ToString());
        }
    }

    /*
      if (Application.isEditor)
        {
#if UNITY_EDITOR
            ItchClient.EditorApiKey = "Your API Key, For Use In Editor";
            ItchClient.StartItchApi(true, "YOUR GAME ID");

            Debug.Log("Have Donwload Key: " + ItchClient.HaveDownloadKey("EXEMPLE USERID: 06452").ToString());
            Debug.Log("Have Purchased: " + ItchClient.UserPurchase("EXEMPLE USERID: 06452", false).ToString());
            Debug.Log("Is A DONATOR: " + ItchClient.UserPurchase("EXEMPLE USERID: 06452", true).ToString());
#endif
        }
        else
        {
            ItchClient.StartItchApi(false);

            Debug.Log("UserId : " + ItchClient.GetMyUserId());
            Debug.Log("UserName : " + ItchClient.GetMyUserName());
            Debug.Log("UserImage : " + ItchClient.GetMyUserImg().ToString());
        } 
     */
}