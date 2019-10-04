#region License
/* * * * *
 * The MIT License (MIT)
 * 
 * Copyright (c) 2017-2019 Darckcomsoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using darckcomsoft.SimpleJSON;
using System;
using System.Net;
using System.Text;

namespace darckcomsoft.itch
{
    public class ItchAPi : MonoBehaviour
    {
		public static ItchAPi Instance;
		Dictionary<DataType, object> PlayerData = new Dictionary<DataType, object>();
        string ClientApiReturn;
        public string GameId = "000000";
        public string ServerApiKey = "key on server side, only put this on runtime";
        public string EditorApiKey = "Add your api key, if this is in editor";

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		public static void StartItchApi()
		{
			if (Instance == null)
			{
				GameObject ItchManager = new GameObject("ItchManager");
				ItchManager.AddComponent<ItchAPi>();
				Instance = ItchManager.GetComponent<ItchAPi>();
			}
			else
			{
				Debug.LogError("ItchApi is allready instantiated, you can use (ItchAPi.Instance), to get ItchApi class");
			}
		}

        public void SetupApi(bool IsEditor, string gameId, string serverapi)
        {
            GameId = gameId;
            ServerApiKey = serverapi;

            LinkItch(IsEditor);
        }

        public void SetupApi(bool IsEditor, string gameId)
        {
            GameId = gameId;

            LinkItch(IsEditor);
        }

        public void SetupApi(bool IsEditor)
        {
            LinkItch(IsEditor);
        }

        public void Clear()
        {
            PlayerData.Clear();
            ClientApiReturn = "";
        }

        #region Link
        public void LinkItch(bool iseditor = false)
        {
            if (iseditor)
            {
				WWW www = new WWW("https://itch.io/api/1/" + EditorApiKey + "/me");
                StartCoroutine(RequestApi(www));
            }
            else
            {
                ClientApiReturn = System.Environment.GetEnvironmentVariable("ITCHIO_API_KEY");
                WWW www = new WWW("https://itch.io/api/1/jwt/me", (byte[])null, new Dictionary<string, string>() { { "Authorization", ClientApiReturn } });
				StartCoroutine(RequestApi(www));
            }
        }

        IEnumerator RequestApi(WWW www)
        {
            yield return (object)www;

            if (www.error == null)
            {
                Datajson(www.text);
            }
            else
            {
                Debug.Log((object)("Error: " + www.error));
            }
        }

        private void Datajson(string jsonString)
        {
            JSONNode jsonNode = JSON.Parse(jsonString);

            PlayerData.Add(DataType.UserId, jsonNode["user"]["id"].Value);
            PlayerData.Add(DataType.UserName, jsonNode["user"]["display_name"].Value);
            PlayerData.Add(DataType.ProfileImage, jsonNode["user"]["cover_url"].Value);
            PlayerData.Add(DataType.dev, jsonNode["user"]["developer"].Value);
            PlayerData.Add(DataType.press, jsonNode["user"]["press_user"].Value);
        }

        #endregion

        #region GetFunctions
        /// <summary>
        /// Check if you are playing on desktop, itch.io app
        /// </summary>
        /// <returns></returns>
        public bool ItchApp()
        {
            ClientApiReturn = System.Environment.GetEnvironmentVariable("ITCHIO_API_KEY");
            return ClientApiReturn != null;
        }

        /// <summary>
        /// Get your itch.io profile id
        /// </summary>
        /// <returns></returns>
        public string GetMyUserId()
        {
            return (string)PlayerData[DataType.UserId];
        }

        /// <summary>
        /// Get your itch.io Display Name.
        /// </summary>
        /// <returns></returns>
        public string GetMyUserName()
        {
            return (string)PlayerData[DataType.UserName];
        }

        /// <summary>
        /// Get your itch.io profile url
        /// </summary>
        /// <returns></returns>
        public string GetMyProfileUrl()
        {
            return (string)PlayerData[DataType.ProfileUrl];
        }

        /// <summary>
        /// Used for server, verify if this user have a download key, to play the game
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool HaveDownloadKey(string userid)
        {
            ReturnValues returnstruc = new ReturnValues();
            ClientApiReturn = System.Environment.GetEnvironmentVariable("ITCHIO_API_KEY");
            WWW www = new WWW("https://itch.io/api/1/" + ServerApiKey + "/game/" + GameId + "/download_keys?user_id=" + userid);
            StartCoroutine(downloadkey(www, returnstruc));

            return returnstruc.boolean;
        }

        public delegate void Callback(bool data);

        /// <summary>
        /// Used for server verify, if this user Owne your game.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool UserPurchase(string userid, bool IsDonate)
        {
            bool returvalue = true;
            ClientApiReturn = System.Environment.GetEnvironmentVariable("ITCHIO_API_KEY");
            WWW www = new WWW("https://itch.io/api/1/" + ServerApiKey + "/game/" + GameId + "/purchases?user_id=" + userid);

            StartCoroutine(purchase(www,IsDonate ,callBack => {
                returvalue = callBack;
                print(returvalue);
            }));

            return returvalue;
        }

        /// <summary>
        /// Get your itch.io profile image
        /// </summary>
        /// <returns></returns>
        public Texture2D GetMyUserImg()
        {
            ReturnValues returnstruc = new ReturnValues();
            WWW www = new WWW((string)PlayerData[DataType.ProfileImage]);
            StartCoroutine(Img(www, returnstruc));

            if (returnstruc.Image != null)
            {
                return returnstruc.Image;
            }
            else
            {
                Debug.LogError("Image Is Null!");
                return null;
            }
        }

        static IEnumerator downloadkey(WWW www, ReturnValues returnbool)
        {
            yield return www;

            if (www.error == null)
            {
                JSONNode jsonNode = JSON.Parse(www.text);

                if (jsonNode["errors"])
                {
                    string errorapi = jsonNode["errors"].Value;

                    if (errorapi == "invalid download key" || errorapi == "no download key found")
                    {
                        returnbool.boolean = false;
                    }
                    else
                    {
                        returnbool.boolean = true;
                    }
                }
                else if (jsonNode["download_key"])
                {
                    returnbool.boolean = true;
                }
            }
            else
            {
                Debug.Log("ERROR: " + www.error);
            }
        }

        static IEnumerator purchase(WWW www, bool isdonated, System.Action<bool> done)
        {
            yield return www;

            if (www.error == null)
            {
                JSONNode jsonNode = JSON.Parse(www.text);

                if (jsonNode["purchases"][0]["status"] || jsonNode["purchases"][0]["donation"])
                {
                    if (isdonated == true)
                    {
                        if (jsonNode["purchases"][0]["donation"].Value == "true")
                        {
                            done(true);
                        }
                        else
                        {
                            done(false);
                            //returnbool.ErrorCustom = "donation:false";
                        }
                    }
                    else
                    {
                        if (jsonNode["purchases"][0]["status"].Value == "complete")
                        {
                            done(true);
                        }
                        else
                        {
                            done(false);
                            //returnbool.ErrorCustom = "purchases_status:panding";
                        }
                    }
                }
                else
                {
                    done(false);
                    //returnbool.ErrorCustom = "purchased:false";
                }
            }
            else
            {
                done(false);
                Debug.Log("ERROR: " + www.error);
            }
        }

        static IEnumerator Img(WWW www, ReturnValues imageretur)
        {
            yield return www;

            if (www.error == null)
            {
                imageretur.Image = www.texture;
            }
            else
            {
                Debug.Log("ERROR: " + www.error);
            }
        }
		#endregion

	}

	public struct ReturnValues
    {
        public Texture2D Image;
        public bool boolean;
        public string ErrorCustom;
    }

    public enum DataType : byte
    {
        none, UserId, UserName, ProfileImage, ProfileUrl, dev, press
    }
}