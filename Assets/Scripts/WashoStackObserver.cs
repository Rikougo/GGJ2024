using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WashoStackObserver : MonoBehaviour
{
    [SerializeField] private GameObject m_entry;
    [SerializeField] private CharacterMovement m_character;

    private List<GameObject> m_entries;
    private int m_currentStack;

    private void Awake()
    {
        m_currentStack = 0;
        m_entries = new List<GameObject>();
    }

    private void OnEnable()
    {
        m_character.OnStackChanged += this.Repaint;
    }

    private void OnDisable()
    {
        m_character.OnStackChanged -= this.Repaint;
    }

    private void Repaint(int p_amount)
    {
        m_currentStack = p_amount;
        foreach (GameObject l_gameObject in m_entries)
        {
            Destroy(l_gameObject);
        }

        m_entries.Clear();
        for (int l_index = 0; l_index < m_currentStack; l_index++)
        {
            m_entries.Add(Instantiate(m_entry, this.transform));
        }
    }
}
