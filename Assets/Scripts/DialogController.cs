using System;
using System.Collections.Generic;
using System.Linq;
using CheapDialogSystem.Runtime.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [Serializable]
    public struct AvatarEntry
    {
        public string name;
        public Sprite avatar;
    }

    [SerializeField] private AvatarEntry[] m_avatarEntries;

    [SerializeField] private GameObject m_layout;
    [SerializeField] private Image m_avatarDisplay;
    [SerializeField] private TMP_Text m_dialogText;
    [SerializeField] private AudioSource m_audioPlayer;

    private DialogContainer m_currentDialog;
    private DialogNodeData m_currentNode;
    private Action m_onEndCallback;

    public void QueueDialog(DialogContainer p_container)
    {
        m_currentDialog = p_container;
    }

    public void ShowNext()
    {
        List<DialogNodeData> l_choices = m_currentDialog.GetChoices(m_currentNode);
        if (l_choices.Count == 0)
        {
            m_onEndCallback?.Invoke();
            this.HideLayout();
            return;
        }

        m_currentNode = l_choices.First();
        this.UpdateDialog();
    }

    public void StartDialog(Action p_onEndCallback)
    {
        m_onEndCallback = p_onEndCallback;
        m_currentNode = m_currentDialog.EntryPoint;

        this.UpdateDialog();
        this.ShowLayout();
    }

    private void UpdateDialog()
    {
        m_audioPlayer.Stop();
        m_audioPlayer.clip = m_currentNode.Sound;
        m_audioPlayer.Play();
        m_avatarDisplay.sprite =
            m_avatarEntries.First(p_entry => p_entry.name.Equals(m_currentNode.DialogTitle)).avatar;
        m_dialogText.text = m_currentNode.DialogText;
    }

    private void ShowLayout()
    {
        this.m_layout.SetActive(true);
    }

    private void HideLayout()
    {
        this.m_layout.SetActive(false);
    }
}