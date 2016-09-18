using UnityEngine;
using System.Collections;

public class TransformMagmaPhoenix : TCallback 
{
	public override void onCall()
	{
		BasicEnemy magma = BasicEnemy.sEnemies[0] as BasicEnemy;
		magma.getSprite().animationCompleteDelegate = null;
		magma.changeAnimation("katia_transform",delegate(tk2dAnimatedSprite sprite, int clipId){magma.spawningComplete = true;magma.changeAnimation("idle");});
	}
}