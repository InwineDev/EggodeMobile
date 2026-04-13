using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

        public class SussyMode : MonoBehaviour
        {
            public Texture2D t2d;

            public Sprite sp;

            private void rechange()
            {
                MeshRenderer[] meshRendererArray = Object.FindObjectsOfType<MeshRenderer>();
                for (int i = 0; i < (int)meshRendererArray.Length; i++)
                {
                    meshRendererArray[i].material.mainTexture = t2d;
                }
                SpriteRenderer[] spriteRendererArray = Object.FindObjectsOfType<SpriteRenderer>();
                for (int j = 0; j < (int)spriteRendererArray.Length; j++)
                {
                    spriteRendererArray[j].sprite = sp;
                }
                Image[] imageArray = Object.FindObjectsOfType<Image>();
                for (int k = 0; k < (int)imageArray.Length; k++)
                {
                    imageArray[k].sprite = this.sp;
                }
            base.Invoke("rechange", 0.01f);
        }

            public void Start()
            {
                base.Invoke("rechange", 0.01f);
            }
        }
