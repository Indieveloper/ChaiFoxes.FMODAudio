﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ChaiFoxes.FMODAudio.Studio
{
	/// <summary>
	/// Studio sound listener in 3D space. Listens to positioned 3D sounds.
	/// 
	/// NOTE: Do not mess with low-level listeners if you're using those.
	/// It *probably* should be fine, but you're doing that at your own risk.
	/// </summary>
	public class StudioListener3D
	{
		/// <summary>
		/// List of all studio listeners. Used to keep track of indices.
		/// </summary>
		private static List<StudioListener3D> _listeners = new List<StudioListener3D>();

		/// <summary>
		/// Listener index. Used to communicate with low-level API.
		/// </summary>
		private int _index;

		/// <summary>
		/// Listener's 3D Attributes.
		/// </summary>
		public Attributes3D Attributes
		{
			get
			{
				AudioMgr.FMODStudioSystem.getListenerAttributes(_index, out FMOD.ATTRIBUTES_3D attributes);

				return attributes.ToAttributes3D();
			}
			set
			{
				AudioMgr.FMODStudioSystem.setListenerAttributes(_index, value.ToFmodAttributes());
			}
		}

		private StudioListener3D()
		{
			_index = _listeners.Count;
			_listeners.Add(this);
			AudioMgr.FMODStudioSystem.setNumListeners(_listeners.Count);

			Attributes = new Attributes3D
			{
				position = Vector3.Zero,
				velocity = Vector3.Zero,
				forwardVector = Vector3.UnitY,
				upVector = Vector3.UnitZ
			};
		}

		/// <summary>
		/// Creates a new listener.
		/// </summary>
		public static StudioListener3D Create()
		{
			return new StudioListener3D();
		}

		/// <summary>
		/// Destroys the listener.
		/// </summary>
		public void Destroy()
		{
			if (_index != _listeners.Count - 1)
			{
				// Listener's index is its only link to its low-level attributes.
				// We don't have access to actual low-level listener objects -
				// if they even exist - and have to resort to this.
				// Basically, if user decides to destroy a listener from the middle of
				// listener list, we take the very last listener and place it instead
				// of the destoyed listener.

				var last = _listeners[_listeners.Count - 1];

				// Saving attribute.
				var attributes = Attributes;
				// Replacing index.
				last._index = _index;
				// Copying attributes over.
				Attributes = attributes;

				// Changing last element's place in listener list.
				_listeners.RemoveAt(_listeners.Count - 1);
				_listeners.Insert(last._index, last);
			}

			_listeners.Remove(this);
			AudioMgr.FMODStudioSystem.setNumListeners(_listeners.Count);

			_index = -1;
		}
	}
}
