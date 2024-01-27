using System;
using System.Linq;
using UnityEngine;

namespace BloodTribute.Utils
{
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] m_spriteSheet;
        [SerializeField] private float m_framePerSecond = 30.0f;
        [SerializeField] private bool m_loop = false;
        [SerializeField] private bool m_hideOnStop = false;

        private int m_currentIndex;

        private float m_elapsed;
        private SpriteRenderer m_sprite;

        private bool m_playing;
        

        public event Action AnimationEnded;

        public float FrameRate
        {
            get => m_framePerSecond;
            set
            {
                m_framePerSecond = value;
            }
        }

        private void Awake()
        {
            m_sprite = GetComponent<SpriteRenderer>();
            m_playing = false;
        }

        public void Play()
        {
            m_currentIndex = 0;
            m_playing = true;
            m_sprite.enabled = true;
            m_sprite.sprite = m_spriteSheet[m_currentIndex];
            m_elapsed = 0.0f;
        }

        public void Stop()
        {
            m_playing = false;

            if (m_hideOnStop)
            {
                this.Hide();
            }
        }

        public void Hide()
        {
            m_sprite.enabled = false;
        }

        private void Update()
        {
            if (m_playing)
            {
                if (m_currentIndex < m_spriteSheet.Length)
                {
                    m_elapsed += Time.deltaTime;

                    if (m_elapsed > 1.0f / m_framePerSecond)
                    {
                        m_currentIndex++;

                        if (m_currentIndex == m_spriteSheet.Length) return;
                        m_sprite.sprite = m_spriteSheet[m_currentIndex];
                        m_elapsed = 0.0f;
                    }
                }
                else
                {
                    this.AnimationEnded?.Invoke();
                    if (m_loop)
                    {
                        m_currentIndex = 0;
                    }
                    else
                    {
                        this.Stop();
                    }
                }
            }
        }
    }
}