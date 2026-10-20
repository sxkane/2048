using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2048.Scripts.Views
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI text;

        public void SetTile(int score, Color tileColor)
        {
            background.color = tileColor;
            text.text = score.ToString();
        }
        
        public void MoveTile(Vector3 position, Action callBack = null)
        {
            StartCoroutine(MoveTo(transform, position, 0.15f, callBack));
        }

        public void SpawnTile(Vector3 position)
        {
            transform.position = position;
            transform.localScale = Vector3.zero;
            
            StartCoroutine(SpawnAnimation());
        }

        private IEnumerator MoveTo( Transform current, Vector3 destination, float duration, Action callBack)
        {
            Vector3 start = current.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                current.position = Vector3.Lerp(start, destination, t);
                yield return null;
            }

            current.position = destination;
            callBack?.Invoke();
        }

        public void PlayMergeAnimation()
        {
            StartCoroutine(MergeAnimation());
        }

        private IEnumerator SpawnAnimation()
        {
            float duration = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                transform.localScale = Vector3.one * t;

                yield return null;
            }

            transform.localScale = Vector3.one;
        }
        
        private IEnumerator MergeAnimation()
        {
            float duration = 0.1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float scale = Mathf.Lerp(1f, 1.2f, t);
                transform.localScale = Vector3.one * scale;

                yield return null;
            }

            transform.localScale = Vector3.one;
        }
    }
}