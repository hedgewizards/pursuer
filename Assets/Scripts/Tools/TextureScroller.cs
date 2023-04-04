using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    Vector2 uvOffset = Vector2.zero;
    public Vector2 uvAnimationRate = new Vector2(0.0f, 1.0f);
    string textureName = "_MainTex";
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if( rend.enabled)
        {
            rend.material.SetTextureOffset(textureName, uvOffset);
        }
    }
}
