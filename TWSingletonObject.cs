using System;

namespace TW
{

    public class TWSingletonObject<T> where T : new()
    {
        private static T _instance = default(T);
        internal static object Singleton_Lock = new object();

        public  static T ShareInstance
        {
            get
            {
                if (TWSingletonObject<T>._instance == null)
                {
                    lock (Singleton_Lock)
                    {
                        if (TWSingletonObject<T>._instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return TWSingletonObject<T>._instance;
            }
        }
        protected TWSingletonObject()
        {
        }
    }
}