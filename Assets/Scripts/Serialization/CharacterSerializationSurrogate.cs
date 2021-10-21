using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public sealed class CharacterSerializationSurrogate : ISerializationSurrogate {
	public void GetObjectData (object obj, SerializationInfo info, StreamingContext context) {
		Character c = (Character)obj;
		info.AddValue ("id", c.id);
		info.AddValue ("name", c.name);
		//info.AddValue ("speed", c.speed);
		info.AddValue ("race", c.race);
		info.AddValue ("alignment", c.alignment);
		info.AddValue ("mapPositions", c.mapPositions);
		info.AddValue ("sightDistance", c.sightDistance);
		info.AddValue ("sightWidth", c.sightWidth);
		info.AddValue ("faceDirection", c.faceDirection);
		info.AddValue ("level", c.level);
		info.AddValue ("hp", c.hp);
		info.AddValue ("maxhp", c.maxhp);
		info.AddValue ("ap", c.ap);
		//info.AddValue ("maxap", c.maxap);
		info.AddValue ("fp", c.fp);
		info.AddValue ("maxfp", c.maxfp);
		info.AddValue ("exp", c.exp);
		info.AddValue ("skills", c.skills);
		info.AddValue ("engagedTargetIDs", c.engagedTargetIDs);
		info.AddValue ("inventory", c.inventory);
		info.AddValue ("controllerType", c.controllerType.FullName);
		info.AddValue ("characterInfo", c.characterInfo);
	}

	public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
		Character c = Character.CreateCharacter ();
		c.controllerType = System.Type.GetType (info.GetString ("controllerType"));
		c.race = (Character.Race)info.GetValue ("race", typeof(Character.Race));
		c.Init ();
		c.id = info.GetUInt16 ("id");
		c.name = info.GetString ("name");
		//c.speed = (float)info.GetValue ("speed", typeof(float));
		c.alignment = (Character.Alignment)info.GetValue ("alignment", typeof(Character.Alignment));
		c.mapPositions = (List<Character.Position>)info.GetValue ("mapPositions", typeof(List<Character.Position>));
		c.sightDistance = (float)info.GetValue ("sightDistance", typeof(float));
		c.sightWidth = (float)info.GetValue ("sightWidth", typeof(float));
		c.faceDirection = (Map.Direction)info.GetValue ("faceDirection", typeof(Map.Direction));
		c.level = info.GetInt32 ("level");
		c.hp = info.GetInt32 ("hp");
		c.maxhp = info.GetInt32 ("maxhp");
		c.ap = info.GetInt32 ("ap");
		//c.maxap = info.GetInt32 ("maxap");
		c.fp = info.GetInt32 ("fp");
		c.maxfp = info.GetInt32 ("maxfp");
		c.exp = info.GetInt32 ("exp");
		c.skills = (List<string>)info.GetValue ("skills", typeof (List<string>));
		c.engagedTargetIDs = (List<ushort>)info.GetValue ("engagedTargetIDs", typeof(List<ushort>));
		c.inventory = (Inventory)info.GetValue ("inventory", typeof(Inventory));
		c.gameObject.AddComponent (c.controllerType);
		c.characterInfo = (Library<object>)info.GetValue ("characterInfo", typeof(Library<object>));
		obj = (object)c;
		//return null;
		return (object)c;
	}
}