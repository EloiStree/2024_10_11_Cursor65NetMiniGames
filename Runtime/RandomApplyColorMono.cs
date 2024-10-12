using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomApplyColorMono : MonoBehaviour
{

    public Renderer m_renderer;
    public Color m_color;

    public void Awake()
    {
        ApplyColorOnRenderer();
    }

    [ContextMenu("Apply Color")]
    public void ApplyColorOnRenderer()
    {
        m_renderer.material.color = m_color;
    }

    private void Reset()
    {
        m_renderer = GetComponent<Renderer>();
        SetRandomNewColor();
        ApplyColorOnRenderer();
    }

    [ContextMenu("Randomize Color")]
    public void SetRandomNewColor()
    {
        m_color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}
