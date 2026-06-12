using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class ColorFlasher : MonoBehaviour
    {
        public void Flash(Color color, float duration, bool includeChildren)
        {
            StartCoroutine(FlashRoutine(color, duration, includeChildren));
        }

        private IEnumerator FlashRoutine(Color color, float duration, bool includeChildren)
        {
            SpriteRenderer[] sprites = includeChildren ? GetComponentsInChildren<SpriteRenderer>() : GetComponents<SpriteRenderer>();
            Renderer[] renderers = includeChildren ? GetComponentsInChildren<Renderer>() : GetComponents<Renderer>();
            Color[] spriteColors = new Color[sprites.Length];
            MaterialPropertyBlock block = new();

            for (int i = 0; i < sprites.Length; i++)
            {
                spriteColors[i] = sprites[i].color;
                sprites[i].color = color;
            }

            foreach (Renderer renderer in renderers)
            {
                if (renderer is SpriteRenderer)
                    continue;

                renderer.GetPropertyBlock(block);
                block.SetColor("_BaseColor", color);
                block.SetColor("_Color", color);
                renderer.SetPropertyBlock(block);
            }

            yield return new WaitForSeconds(duration);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                    sprites[i].color = spriteColors[i];
            }

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null && renderer is not SpriteRenderer)
                    renderer.SetPropertyBlock(null);
            }
        }
    }
}
