using UnityEngine;
using System.Collections;

namespace SoulAvenger
{
	public class CollisionDetection
	{
		public static bool Test2D(Transform[] ts,BoxCollider b)
		{
			foreach(Transform t in ts)
			{
				BoxCollider a = t.GetComponent<BoxCollider>();
				if(a!=null)
				{
					if(Test2D(a,b))
						return true;
				}
			}
			return false;
		}
		
		public static Bounds getBound(BoxCollider box)
		{
			return new Bounds(box.transform.localToWorldMatrix.MultiplyPoint(new Vector3(box.center.x,box.center.y,0.0f)),box.size);
		}
		
		public static bool Test2D(BoxCollider a,BoxCollider b)
		{
			Bounds boundA = getBound(a);
			Bounds boundB = getBound(b);
			
			return boundA.Intersects(boundB);
		}
		
		public static bool Test2D(BoxCollider a,BoxCollider b,ref Vector3 middle)
		{
			Bounds boundA = getBound(a);
			Bounds boundB = getBound(b);
			
			middle = (boundA.center + boundB.center)*0.5f;
						
			return boundA.Intersects(boundB);
		}
	}
};