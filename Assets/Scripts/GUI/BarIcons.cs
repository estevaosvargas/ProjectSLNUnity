using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarIcons : MonoBehaviour
{

    public Sprite FullSprite;
    public Sprite HalfSprite;
    public Sprite EmptySprite;

    public Image[] IconsSprites;


    void Start()
    {

    }


    public void RefreshBar(float HP)
    {
        if (HP >= 0)
        {
            #region Sprites
            IconsSprites[0].sprite = EmptySprite;
            IconsSprites[1].sprite = EmptySprite;
            IconsSprites[2].sprite = EmptySprite;
            IconsSprites[3].sprite = EmptySprite;
            IconsSprites[4].sprite = EmptySprite;
            IconsSprites[5].sprite = EmptySprite;
            IconsSprites[6].sprite = EmptySprite;
            IconsSprites[7].sprite = EmptySprite;
            #endregion

            if (HP >= 5)
            {
                #region Sprites
                IconsSprites[0].sprite = HalfSprite;
                IconsSprites[1].sprite = EmptySprite;
                IconsSprites[2].sprite = EmptySprite;
                IconsSprites[3].sprite = EmptySprite;
                IconsSprites[4].sprite = EmptySprite;
                IconsSprites[5].sprite = EmptySprite;
                IconsSprites[6].sprite = EmptySprite;
                IconsSprites[7].sprite = EmptySprite;
                #endregion

                if (HP >= 10)
                {
                    #region Sprites
                    IconsSprites[0].sprite = FullSprite;
                    IconsSprites[1].sprite = EmptySprite;
                    IconsSprites[2].sprite = EmptySprite;
                    IconsSprites[3].sprite = EmptySprite;
                    IconsSprites[4].sprite = EmptySprite;
                    IconsSprites[5].sprite = EmptySprite;
                    IconsSprites[6].sprite = EmptySprite;
                    IconsSprites[7].sprite = EmptySprite;
                    #endregion

                    if (HP >= 15)
                    {
                        #region Sprites
                        IconsSprites[0].sprite = FullSprite;
                        IconsSprites[1].sprite = HalfSprite;
                        IconsSprites[2].sprite = EmptySprite;
                        IconsSprites[3].sprite = EmptySprite;
                        IconsSprites[4].sprite = EmptySprite;
                        IconsSprites[5].sprite = EmptySprite;
                        IconsSprites[6].sprite = EmptySprite;
                        IconsSprites[7].sprite = EmptySprite;
                        #endregion

                        if (HP >= 20)
                        {
                            #region Sprites
                            IconsSprites[0].sprite = FullSprite;
                            IconsSprites[1].sprite = FullSprite;
                            IconsSprites[2].sprite = EmptySprite;
                            IconsSprites[3].sprite = EmptySprite;
                            IconsSprites[4].sprite = EmptySprite;
                            IconsSprites[5].sprite = EmptySprite;
                            IconsSprites[6].sprite = EmptySprite;
                            IconsSprites[7].sprite = EmptySprite;
                            #endregion

                            if (HP >= 25)
                            {
                                #region Sprites
                                IconsSprites[0].sprite = FullSprite;
                                IconsSprites[1].sprite = FullSprite;
                                IconsSprites[2].sprite = HalfSprite;
                                IconsSprites[3].sprite = EmptySprite;
                                IconsSprites[4].sprite = EmptySprite;
                                IconsSprites[5].sprite = EmptySprite;
                                IconsSprites[6].sprite = EmptySprite;
                                IconsSprites[7].sprite = EmptySprite;
                                #endregion

                                if (HP >= 30)
                                {
                                    #region Sprites
                                    IconsSprites[0].sprite = FullSprite;
                                    IconsSprites[1].sprite = FullSprite;
                                    IconsSprites[2].sprite = FullSprite;
                                    IconsSprites[3].sprite = EmptySprite;
                                    IconsSprites[4].sprite = EmptySprite;
                                    IconsSprites[5].sprite = EmptySprite;
                                    IconsSprites[6].sprite = EmptySprite;
                                    IconsSprites[7].sprite = EmptySprite;
                                    #endregion

                                    if (HP >= 35)
                                    {
                                        #region Sprites
                                        IconsSprites[0].sprite = FullSprite;
                                        IconsSprites[1].sprite = FullSprite;
                                        IconsSprites[2].sprite = FullSprite;
                                        IconsSprites[3].sprite = HalfSprite;
                                        IconsSprites[4].sprite = EmptySprite;
                                        IconsSprites[5].sprite = EmptySprite;
                                        IconsSprites[6].sprite = EmptySprite;
                                        IconsSprites[7].sprite = EmptySprite;
                                        #endregion

                                        if (HP >= 40)
                                        {
                                            #region Sprites
                                            IconsSprites[0].sprite = FullSprite;
                                            IconsSprites[1].sprite = FullSprite;
                                            IconsSprites[2].sprite = FullSprite;
                                            IconsSprites[3].sprite = FullSprite;
                                            IconsSprites[4].sprite = EmptySprite;
                                            IconsSprites[5].sprite = EmptySprite;
                                            IconsSprites[6].sprite = EmptySprite;
                                            IconsSprites[7].sprite = EmptySprite;
                                            #endregion

                                            if (HP >= 45)
                                            {
                                                #region Sprites
                                                IconsSprites[0].sprite = FullSprite;
                                                IconsSprites[1].sprite = FullSprite;
                                                IconsSprites[2].sprite = FullSprite;
                                                IconsSprites[3].sprite = FullSprite;
                                                IconsSprites[4].sprite = HalfSprite;
                                                IconsSprites[5].sprite = EmptySprite;
                                                IconsSprites[6].sprite = EmptySprite;
                                                IconsSprites[7].sprite = EmptySprite;
                                                #endregion

                                                if (HP >= 50)
                                                {
                                                    #region Sprites
                                                    IconsSprites[4].sprite = FullSprite;
                                                    IconsSprites[5].sprite = EmptySprite;
                                                    IconsSprites[6].sprite = EmptySprite;
                                                    IconsSprites[7].sprite = EmptySprite;
                                                    #endregion

                                                    if (HP >= 55)
                                                    {
                                                        #region Sprites
                                                        IconsSprites[5].sprite = HalfSprite;
                                                        IconsSprites[6].sprite = EmptySprite;
                                                        IconsSprites[7].sprite = EmptySprite;
                                                        #endregion

                                                        if (HP >= 60)
                                                        {
                                                            #region Sprites
                                                            IconsSprites[5].sprite = FullSprite;
                                                            IconsSprites[6].sprite = EmptySprite;
                                                            IconsSprites[7].sprite = EmptySprite;
                                                            #endregion
                                                            if (HP >= 65)
                                                            {
                                                                #region Sprites
                                                                IconsSprites[6].sprite = HalfSprite;
                                                                IconsSprites[7].sprite = EmptySprite;
                                                                #endregion

                                                                if (HP >= 70)
                                                                {
                                                                    #region Sprites
                                                                    IconsSprites[6].sprite = FullSprite;
                                                                    IconsSprites[7].sprite = EmptySprite;
                                                                    #endregion

                                                                    if (HP >= 75)
                                                                    {
                                                                        #region Sprites
                                                                        IconsSprites[7].sprite = HalfSprite;
                                                                        #endregion
                                                                        if (HP >= 80)
                                                                        {
                                                                            #region Sprites
                                                                            IconsSprites[7].sprite = FullSprite;
                                                                            #endregion

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}