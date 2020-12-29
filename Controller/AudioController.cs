/**************************************************
* 创建作者：	咕咕咕
* 创建时间：	2020-12-29
* 作用描述：	#
***************************************************/

using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

namespace SpectrumView
{
	public class AudioController : MonoBehaviour
	{
		#region ----字段----
		public AudioView view;
		public Spectrum2DView sview;
		public ColorGLView lowView;
		public ColorGLView highView;

		//FFT
		private static readonly object lockObj = new object();
		private const int freqLength = 22;
		//采样频段 /*-*/
		private static readonly int[] FREQUENCY = new int[freqLength] { 40, 80, 90, 130, 180, 220, 260, 
																		330, 480, 570, 660, 770, 880, 
																		950, 1080, 1500, 2000, 3000,
																		5000, 8000, 10000, 16000 };
		float[] data = new float[freqLength];
        #endregion

        #region ----monoBehaviour----
        private void Awake()
        {
			view.RegisterSlider(OnChangePlayTime);
			lowView.Register(OnChangeLow);
			highView.Register(OnChangeHigh);
			lowView.InitColor(sview.lowColor);
			highView.InitColor(sview.highColor);
			Load("Sounds/2");
		}
        #endregion

        #region ----公有方法----
		public void Load(string path)
        {
			AudioClip clip = Resources.Load<AudioClip>(path);
			OnLoadClip(clip);
        }
		#endregion

		#region ----私有方法----
		private void OnLoadClip(AudioClip clip)
		{
			if (clip == null)
			{
				sview.Show(false);
				view.Show(false);
				return;
			}
			view.Show(true);
			sview.Show(true);
			lowView.Show(true);
			highView.Show(true);
			view.PlayClip(clip);
		}

		private void OnChangePlayTime(float progress)
		{
			if (progress <= 0)
				progress = 0;
			if (progress >= 1)
				progress = 0.99f;
			view.OnChangePlayTime(progress);
		}

		private void OnChangeLow(Color c) => sview.lowColor = c;

		private void OnChangeHigh(Color c) => sview.highColor = c;
		#endregion

		#region ----FFT----
		public float[] GetDataSafe()
		{
			float[] nd = new float[freqLength];
			lock (lockObj)
			{
				data.CopyTo(nd, 0);
			}
			return nd;
		}

		void SetDataSafe(float[] nd)
		{
			//这里不能修改nd的数据，否则后果很严重，兄děī
			lock (lockObj)
			{
				nd.CopyTo(data, 0);
			}
			sview.SetDatas(data);
		}

        private void OnAudioFilterRead(float[] data, int channels)
		{
			try
			{
				float[] sdata = new float[data.Length / channels];
				for (int i = 0; i < sdata.Length; i++)
				{
					sdata[i] = data[i * channels];
				}
				float[] realout = new float[sdata.Length];
				float[] imagout = new float[sdata.Length];
				float[] pamlout = new float[sdata.Length];
				FFT.Compute((uint)sdata.Length, sdata, null, realout, imagout, false);
				FFT.Norm((uint)sdata.Length, realout, imagout, pamlout);
				float[] odata = new float[freqLength];
				int centerFreq = 22050;
				for (int i = 0; i < freqLength; ++i)
				{
					if (FREQUENCY[i] > centerFreq)
					{
						odata[i] = 0;
					}
					else
					{
						int indice = (int)((float)FREQUENCY[i] * (float)pamlout.Length / (float)44100);
						if (indice >= pamlout.Length) indice = pamlout.Length - 1;
						float v = Mathf.Sqrt(pamlout[indice]);
						v = Mathf.Sqrt(v);

						odata[i] = v;
					}
				}
				SetDataSafe(odata);
			}
			catch
			{
				print("FFT err.");
			}
		}
		#endregion

		#region ----静态方法----
		// 秒数 -> 00:00
		public static string TimeFormat(int sec)
		{
			StringBuilder timeStr = new StringBuilder();
			int curTime = sec;
			timeStr.Append(string.Format("{0:D2}", curTime / 60) + ":");
			curTime = curTime % 60;
			timeStr.Append(string.Format("{0:D2}", curTime));

			return timeStr.ToString();
		}
		#endregion
	}
}