using System;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Generic singleton pattern for MonoBehaviour classes
	/// </summary>
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		private static readonly object Lock = new object();
		private static bool ApplicationIsQuitting = false;

		public static T Instance
		{
			get
			{
				if (ApplicationIsQuitting) return null;
				lock (Lock)
				{
					if (instance == null)
					{
						instance = FindAnyObjectByType<T>();
						if (instance == null)
						{
							GameObject singleton = new GameObject($"{typeof(T).Name} (Singleton)");
							instance = singleton.AddComponent<T>();
							DontDestroyOnLoad(singleton);
						}
					}
				}

				return instance;
			}
		}

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
				DontDestroyOnLoad(gameObject);
			}
			else if (instance != this)
			{
				Destroy(gameObject);
			}
		}

		protected void OnDestroy()
		{
			if (instance == this) ApplicationIsQuitting = true;
		}
	}
}