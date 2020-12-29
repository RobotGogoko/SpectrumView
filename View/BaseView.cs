/**************************************************
* 创建作者：	咕咕咕
* 创建时间：	2020-12-29
* 引擎版本：	2020.1.17f1c1
* 作用描述：	#
***************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace SpectrumView
{
    public abstract class BaseView : MonoBehaviour
    {
        #region ----字段----
        private bool isIniCompleted = false;
        #endregion

        #region ----MonoBehaviour----
        void Awake()
        {
            Init();
	    }
        #endregion

        #region ----公有方法----
        public virtual void Init()
        {
            if (isIniCompleted)
            {
                return;
            }
            OnInit();
            isIniCompleted = true;
        }
        public abstract void Show(bool display);
        #endregion

        #region ----私有方法----
        protected virtual void OnInit() { }
        #endregion
    }
}