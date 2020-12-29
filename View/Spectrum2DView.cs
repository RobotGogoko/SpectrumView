/**************************************************
* 创建作者：	咕咕咕
* 创建时间：	2020-12-29
* 引擎版本：	2020.1.17f1c1
* 作用描述：	#频谱2D绘制展示
***************************************************/

using UnityEngine;

namespace SpectrumView
{
    public class Spectrum2DView : BaseView
    {
        #region ----字段----
        [SerializeField] private Vector2 screenPos = new Vector2(480, 320);
        [SerializeField] private float scale = 1;
        [SerializeField] public Color lowColor = Color.cyan;
        [SerializeField] public Color highColor = Color.magenta;

        private Material lineMaterial;
        private float[] spectrumData = new float[1024];
        private Vector3 startPos;
        private float width;
        private float space;
        private int length;
        private float startX;
        private float[] fftDatas;
        #endregion

        #region ----MonoBehaviour----
        private void Start()
        {
            lineMaterial = new Material(Shader.Find("GUI/Text Shader"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

            //绘制的位置的初始化
            width = Screen.width * scale / 2;
            length = spectrumData.Length / 12;
            startX = -width / 2;
            space = width / length;
            startPos = new Vector3(startX + screenPos.x, screenPos.y, 0);
        }

        private void Update() => AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        private void OnRenderObject()
        {
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            DrawSpectrum();     // 绘制实时频谱
            DrawFFT();          // 绘制FFT频段
            GL.PopMatrix();
        }

        private float Limit(float data) => data > 0.12f ? 0.12f : data; 

        private void DrawFFT()
        {
            if (fftDatas == null)
                return;

            float spc = width / fftDatas.Length;
            GL.Begin(GL.LINES);
            for (int i = 0; i < fftDatas.Length - 1; ++i)
            {
                GL.Color(Color.Lerp(lowColor, highColor, i / (float)(fftDatas.Length - 1)));
                GL.Vertex3(startPos.x + i * spc, startPos.y - 130, 0);
                GL.Vertex3(startPos.x + i * spc, startPos.y - 130 + fftDatas[i] * 10, 0);
                GL.Vertex3(startPos.x + i * spc, startPos.y - 130 + fftDatas[i] * 10, 0);
                GL.Vertex3(startPos.x + (i + 1) * spc, startPos.y - 130 + fftDatas[i] * 10, 0);
                GL.Vertex3(startPos.x + (i + 1) * spc, startPos.y - 130 + fftDatas[i] * 10, 0);
                GL.Vertex3(startPos.x + (i + 1) * spc, startPos.y - 130, 0);
            }
            GL.End();
        }

        private void DrawSpectrum()
        {
            GL.Begin(GL.QUADS);
            float data;
            float data2;
            for (int i = 0; i < length - 1; ++i)
            {
                data = Limit(spectrumData[i]);
                data2 = Limit(spectrumData[i + 1]);
                GL.Color(lowColor);
                GL.Vertex3(startPos.x + i * space, startPos.y, startPos.z);
                GL.Vertex3(startPos.x + (i + 1) * space, startPos.y, startPos.z);
                GL.Color(Color.Lerp(lowColor, highColor, (data + data2) / 0.02f));
                GL.Vertex3(startPos.x + (i + 1) * space, startPos.y + data2 * 1200, startPos.z);
                GL.Vertex3(startPos.x + i * space, startPos.y + data * 1200, startPos.z);
            }
            GL.End();
        }
        #endregion

        #region ----外部调用----
        public void SetDatas(float[] datas) => fftDatas = datas;
        #endregion

        #region ----Base----
        public override void Show(bool display) => gameObject.SetActive(display);
        #endregion
    }
}