/**************************************************
* 创建作者：	咕咕咕
* 创建时间：	2020-12-29
* 引擎版本：	2020.1.17f1c1
* 作用描述：	#
***************************************************/

using System;
using UnityEngine;

namespace SpectrumView
{
    public class ColorGLView : BaseView
    {
        #region ----字段----
        [SerializeField] private Vector2 pos;
        [SerializeField] private int drawCount = 360;
        [SerializeField] private float radius = 50;
        [SerializeField] private RectTransform flagTrans;

        private float perAngle;
        private float curAngle;
        private Vector3[] posList;
        private Action<Color> onChangeColor;
        private Material lineMaterial;
        private bool isDown = false;
        #endregion

        #region ----MonoBehaviour----
        private void Start()
        {
            lineMaterial = new Material(Shader.Find("GUI/Text Shader"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

            perAngle = Mathf.PI * 2 / drawCount;
            posList = new Vector3[drawCount + 1];
        }

        private void Update()
        {
            //检测鼠标事件
            if (Input.GetMouseButtonDown(0))
            {
                if (Vector2.Distance(Input.mousePosition, pos) <= radius + 10) isDown = true;
            }
            if (Input.GetMouseButtonUp(0)) isDown = false;
            if (isDown) OnDrag(Input.mousePosition);
        }

        private void OnRenderObject()
        {
            //初始化圆上的所有点
            for (int i = 0; i < drawCount; i++)
            {
                curAngle = perAngle * i;
                posList[i] = new Vector3(Mathf.Sin(curAngle) * radius + pos.x, Mathf.Cos(curAngle) * radius + pos.y, 0);
            }
            posList[drawCount] = new Vector3(Mathf.Sin(0) * radius + pos.x, Mathf.Cos(0) * radius + pos.y, 0);

            //绘制圆，填充颜色
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL.TRIANGLES);
            for (int i = 0; i < posList.Length - 1; i++)
            {
                GL.Color(Color.HSVToRGB(i/(float)drawCount, 0, 1));
                GL.Vertex3(pos.x, pos.y, 0);

                GL.Color(Color.HSVToRGB(i / (float)drawCount, 1, 1));
                GL.Vertex3(posList[i].x, posList[i].y, 0);
                GL.Color(Color.HSVToRGB((i + 1) / (float)drawCount, 1, 1));
                GL.Vertex3(posList[i + 1].x, posList[i + 1].y, 0);
            }
                
            GL.End();
            GL.PopMatrix();
        }

        private void OnChange(Color c) => onChangeColor?.Invoke(c);

        public void Register(Action<Color> action) => onChangeColor += action;

        // 圆上的位置 -> 颜色
        public void OnDrag(Vector2 p2)
        {
            Vector2 p = p2 - pos;
            float s = p.magnitude / radius;
            if (s > 1)
            {
                s = 1;
                flagTrans.anchoredPosition = p.normalized * radius + pos;
            }
            else
            {
                flagTrans.anchoredPosition = p2;
            }
            float h = Mathf.Atan2(p.x, p.y);
            h /= Mathf.PI * 2;
            if (h < 0) h++;

            OnChange(Color.HSVToRGB(h, s, 1));
        }

        // 颜色 -> 圆上的位置
        public void InitColor(Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            h *= Mathf.PI * 2;
            s *= radius;
            flagTrans.anchoredPosition = new Vector2(Mathf.Sin(h) * s, Mathf.Cos(h) * s) + pos;
        }
        #endregion

        #region ----Base----
        public override void Show(bool display) => gameObject.SetActive(true);
        #endregion
    }
}