using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using darckcomsoft.itch;
using System.Text;

public class DcsItchExemple : MonoBehaviour
{
    void Start()
    {
		ItchAPi.StartItchApi();

		if (Application.isEditor)
        {
#if UNITY_EDITOR
			ItchAPi.Instance.EditorApiKey = "5av3kO2VL0iQuBu3zp7TNTbb6b257OYL81R3KQQ6";
			ItchAPi.Instance.ServerApiKey = "5av3kO2VL0iQuBu3zp7TNTbb6b257OYL81R3KQQ6";
			ItchAPi.Instance.SetupApi(true, "122257");

            Debug.Log("Have Donwload Key: " + ItchAPi.Instance.HaveDownloadKey("870669"));
            Debug.Log("Have Purchased: " + ItchAPi.Instance.UserPurchase("870669", false));
            Debug.Log("Is A DONATOR: " + ItchAPi.Instance.UserPurchase("870669", true));
#endif
        }
        else
        {
			ItchAPi.Instance.SetupApi(false);

            Debug.Log("UserId : " + ItchAPi.Instance.GetMyUserId());
            Debug.Log("UserName : " + ItchAPi.Instance.GetMyUserName());
            Debug.Log("UserImage : " + ItchAPi.Instance.GetMyUserImg().ToString());
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