/**************************************************
* 创建作者：	咕咕咕
* 创建时间：	2020-12-29
* 引擎版本：	2020.1.17f1c1
* 作用描述：	#音频播放信息
***************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpectrumView
{
    public class AudioView : BaseView
    {
        #region ----字段----
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private Text timeLabel;
        [SerializeField] private Text curTimeLabel;
        [SerializeField] private Slider progress;
        #endregion

        #region ----MonoBehaviour----
        void Update()
        {
            //更新进度条
            if (audioPlayer.clip != null)
            {
                progress.value = audioPlayer.time / audioPlayer.clip.length;
                curTimeLabel.text = AudioController.TimeFormat(Mathf.FloorToInt(audioPlayer.time));
            }
	    }
        #endregion

        #region ----公有方法----
        public void PlayClip(AudioClip clip)
        {
            audioPlayer.clip = clip;
            timeLabel.text = AudioController.TimeFormat(Mathf.FloorToInt(clip.length));
            audioPlayer.time = 0;
            audioPlayer.Play();
        }

        public void OnChangePlayTime(float progress)
        {
            float time = audioPlayer.clip.length * progress;
            audioPlayer.time = time;
        }

        public void RegisterSlider(Action<float> action) => progress.onValueChanged.AddListener((f) => action(f));
        #endregion

        #region ----Base----
        public override void Show(bool display) => gameObject.SetActive(display);
        #endregion
    }
}