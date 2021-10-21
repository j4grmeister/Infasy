using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public sealed class Vector2SerializationSurrogate : ISerializationSurrogate {
	public void GetObjectData (object obj, SerializationInfo info, StreamingContext context) {
		Vector2 v = (Vector2)obj;
		info.AddValue ("x", v.x);
		info.AddValue ("y", v.y);
	}

	public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
		Vector2 v = (Vector2)obj;
		v.x = (float)info.GetValue ("x", typeof(float));
		v.y = (float)info.GetValue ("y", typeof(float));
		//return null
		return (object)v;
	}
}