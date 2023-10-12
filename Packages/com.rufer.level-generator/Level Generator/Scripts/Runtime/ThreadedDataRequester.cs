using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace LevelGenerator
{
    internal class ThreadedDataRequester : MonoBehaviour
    {
        private static ThreadedDataRequester _instance;

        private Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

        private void Awake()
        {
            _instance = FindObjectOfType<ThreadedDataRequester>();
        }

        private void Update()
        {
            if (_dataQueue.Count > 0)
            {
                for (int i = 0; i < _dataQueue.Count; i++)
                {
                    ThreadInfo threadInfo = _dataQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }

        internal static void RequestData(Func<object> generateData, Action<object> callback)
        {
            ThreadStart threadStart = delegate {
                _instance.DataThread(generateData, callback);
            };

            new Thread(threadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> callback)
        {
            object data = generateData();
            lock (_dataQueue)
            {
                _dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }

        private struct ThreadInfo
        {
            internal readonly Action<object> Callback;
            internal readonly object Parameter;

            internal ThreadInfo(Action<object> callback, object parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }
}