using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

using TW;

//注意，自己接受的事件处理， 必须是此函数类型
public delegate void OnNotificationTrigger(TWNotification notification);

//issue, 暂时没用到 OnNotificationTrigger。 无法解决实例化问题

// 通知中心模块 移植from OC
//1、 解决 MonoBehaviour 脚本之间通信
namespace TW
{
    public class TWNotificationCenter : TWSingletonObject<TWNotificationCenter>
    {
        private List<TWObserve> triggers;
        // private event OnNotificationTrigger NoticeEventHandle; 暂时解决不了分类触发问题
        public TWNotificationCenter()
        {
            triggers = new List<TWObserve>();
        }

        #region  添加删除 通知
        public void AddObserve(UnityEngine.Object obj, string noticeName, string methodHandle)
        {
            TWObserve obv = new TWObserve(obj, noticeName, methodHandle);
            triggers.Add(obv);
        }

        public void RemoveObserve(System.Object obj, string noticeName)
        {
            List<TWObserve> toDel = new List<TWObserve>();
            foreach (TWObserve obv in triggers)
            {
                if (noticeName == null)
                {
                    if (obv.Observe.Equals(obj))
                    {
                        toDel.Add(obv);
                    }
                }
                else if (obv.NoticeName.Equals(noticeName) && obv.Observe.Equals(obj))
                {
                    toDel.Add(obv);
                }
            }
            foreach (TWObserve obv in toDel)
            {
                triggers.Remove(obv);
            }
        }
        #endregion



        #region 发送通知

        public void PostNotification(string name)
        {
            PostNotification(name, null);
        }

        public void PostNotification(string name, UnityEngine.Object info)
        {
            PostNotification(name, info, null);
        }

        public void PostNotification(string name, System.Object info, UnityEngine.Object sender)
        {
            TWNotification notice = new TWNotification(name, info, sender);

            foreach (TWObserve obv in triggers)
            {
                if (obv.NoticeName.Equals(name))
                {
                    Component opent = obv.Observe as Component;
                    //当一个object有多个脚本的时候，效率可能会低
                    opent.SendMessage(obv.MethodHandle, notice);
                }
            }

        }

        #endregion

    }


    public class TWNotification : System.Object
    {
        public string Name;  //事件名
        public System.Object Info; // 可传递的数据

        public UnityEngine.Object Sender; //可选的发送者


        public TWNotification(string name, System.Object info, UnityEngine.Object sender)
        {
            Name = name;
            Info = info;
            Sender = sender;
        }
    }


    public class TWObserve : System.Object
    {
        public UnityEngine.Object Observe;
        public string NoticeName;
        public string MethodHandle;
        // public OnNotificationTrigger MethodHandle;

        public TWObserve(UnityEngine.Object obj, string name, string method)
        {
            Observe = obj;
            NoticeName = name;
            //FindMethodOf(obj, method).Name
            //还是提示不匹配
            //那就只保存method， 使用sendmessage 了
            // MethodHandle = new OnNotificationTrigger(FindMethodOf(obj, method).Name);
            MethodHandle = method;
        }

        public MethodInfo FindMethodOf(System.Object obj, string meth)
        {
            Type type = obj.GetType();
            //获取所有public修饰的方法
            MethodInfo methodInfo = type.GetMethod(meth);
            return methodInfo;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TWObserve tmp = obj as TWObserve;

            return this.Observe.Equals(tmp.Observe) &&
             this.NoticeName.Equals(tmp.NoticeName) &&
             this.MethodHandle.Equals(tmp.MethodHandle);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.NoticeName.GetHashCode() +
            this.Observe.GetHashCode() +
            this.MethodHandle.GetHashCode();
        }
    }
}