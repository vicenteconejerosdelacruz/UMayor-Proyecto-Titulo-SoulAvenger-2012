using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Sprite/tk2dAnimatedSprite")]
/// <summary>
/// Sprite implementation which plays and maintains animations
/// </summary>
public class tk2dAnimatedSprite : tk2dSprite
{
	/// <summary>
	/// <see cref="tk2dSpriteAnimation"/>
	/// </summary>
	public tk2dSpriteAnimation anim;
	/// <summary>
	/// Currently playing/active clip
	/// </summary>
	public int clipId = 0;
	/// <summary>
	/// Interface option to play the animation automatically when instantiated / game is started. Useful for background looping animations.
	/// </summary>
	public bool playAutomatically = false;
	
	/// <summary>
	/// Globally pause all animated sprites
	/// </summary>
	public static bool g_paused = false;
	
	/// <summary>
	/// Pause this animated sprite
	/// </summary>
	public bool paused = false;
	
	/// <summary>
	/// Interface option to create an animated box collider for this animated sprite
	/// </summary>
	public bool createCollider = false;
	
	/// <summary>
	/// Currently active clip
	/// </summary>
	tk2dSpriteAnimationClip currentClip = null;
	
	/// <summary>
	/// Time into the current clip. This is in clip local time (i.e. (int)clipTime = currentFrame)
	/// </summary>
    float clipTime = 0.0f;
	public float getClipTime(){return clipTime;}
	
	/// <summary>
	/// Previous frame identifier
	/// </summary>
	int previousFrame = -1;
	
	/// <summary>
	/// Animation complete delegate 
	/// </summary>
	public delegate void AnimationCompleteDelegate(tk2dAnimatedSprite sprite, int clipId);
	/// <summary>
	/// Animation complete event. This is called when the animation has completed playing. Will not trigger on looped animations
	/// </summary>
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	/// <summary>
	/// Animation event delegate.
	/// </summary>
	public delegate void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum);
	/// <summary>
	/// Animation event. This is called when the frame displayed has <see cref="tk2dSpriteAnimationFrame.triggerEvent"/> set.
	/// The triggering frame is passed to the delegate, and the eventInfo / Int / Float can be extracted from there.
	/// </summary>
	public AnimationEventDelegate animationEventDelegate;
	
	public float localTimeScale = 1.0f;
	
	new void Start()
	{
		base.Start();
		
		if (playAutomatically)
			Play(clipId);
	}
	
	/// <summary>
	/// Play the active clip. Will restart the clip if called again.
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	public void Play()
	{
		Play(clipId);
	}
	
	/// <summary>
	/// Play the active clip, starting "clipStartTime" seconds into the clip. 
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	public void Play(float clipStartTime)
	{
		Play(clipId, clipStartTime);
	}
	
	/// <summary>
	/// Play the specified clip.
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	/// <param name='name'>
	/// Name of clip. Try to cache the animation clip Id and use that instead for performance.
	/// </param>
	public void Play(string name)
	{
		int id = anim?anim.GetClipIdByName(name):-1;
		Play(id);
	}
	
	/// <summary>
	/// Play the specified clip, starting "clipStartTime" seconds into the clip.
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	/// <param name='name'> Name of clip. Try to cache the animation clip Id and use that instead for performance. </param>
	/// <param name='clipStartTime'> Clip start time in seconds. </param>
	public void Play(string name, float clipStartTime)
	{
		int id = anim?anim.GetClipIdByName(name):-1;
		Play(id, clipStartTime);
	}
	
	/// <summary>
	/// The currently active or playing <see cref="tk2dSpriteAnimationClip"/>
	/// </summary>
	public tk2dSpriteAnimationClip CurrentClip
	{
		get { return currentClip; }
	}
	
	/// <summary>
	/// Stop the currently playing clip.
	/// </summary>
	public void Stop()
	{
		currentClip = null;
	}
	
	/// <summary>
	/// Is a clip currently playing?
	/// </summary>
	public bool isPlaying()
	{
		return currentClip != null;
	}
	
	protected override bool NeedBoxCollider()
	{
		return createCollider;
	}
	
	/// <summary>
	/// Resolves an animation clip by name and returns a unique id.
	/// This is a convenient alias to <see cref="tk2dSpriteAnimation.GetClipIdByName"/>
	/// </summary>
	/// <returns>
	/// Unique Animation Clip Id.
	/// </returns>
	/// <param name='name'>Case sensitive clip name, as defined in <see cref="tk2dSpriteAnimationClip"/>. </param>
	public int GetClipIdByName(string name)
	{
		return anim?anim.GetClipIdByName(name):-1;
	}
	
	/// <summary>
	/// Play the clip specified by identifier.
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	/// <param name='id'>
	/// Use <see cref="GetClipIdByName"/> to resolve a named clip id
	/// </param>
	public void Play(int id)
	{
		Play(id, 0.0f);
	}
	
	/// <summary>
	/// Play the clip specified by identifier.
	/// Will restart the clip at clipStartTime if called while the clip is playing.
	/// </summary>
	/// <param name='id'>Use <see cref="GetClipIdByName"/> to resolve a named clip id</param>	
	/// <param name='clipStartTime'> Clip start time in seconds. </param>
	public void Play(int id, float clipStartTime)
	{
		clipId = id;
		if (id >= 0 && anim && id < anim.clips.Length)
		{
			currentClip = anim.clips[id];

			// Simply swap, no animation is played
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || currentClip.frames == null)
			{
				SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
				
				if (currentClip.frames[0].triggerEvent)
				{
					if (animationEventDelegate != null)
						animationEventDelegate( this, currentClip, currentClip.frames[0], 0 );
				}
				currentClip = null;
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int rnd = Random.Range(0, currentClip.frames.Length - 1);
				var currentFrame = currentClip.frames[rnd];
				clipTime = rnd * currentClip.fps;
				
				SwitchCollectionAndSprite(currentFrame.spriteCollection, currentFrame.spriteId);
				if (currentFrame.triggerEvent)
				{
					if (animationEventDelegate != null)
						animationEventDelegate( this, currentClip, currentFrame, 0 );
				}
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame)
				{
					currentClip = null;
					previousFrame = -1;
				}
			}
			else
			{
				// clipStartTime is in seconds
				// clipTime is in clip local time (ignoring fps)
				clipTime = clipStartTime * currentClip.fps;
				previousFrame = -1;
				
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && clipTime >= currentClip.fps * currentClip.frames.Length)
				{
					// force to the last frame
					clipTime = currentClip.fps * (currentClip.frames.Length - 0.1f);
				}
			}
		}
		else
		{
			OnCompleteAnimation();
			currentClip = null;
		}
	}
	
	/// <summary>
	/// Pause the currently playing clip. Will do nothing if the clip is currently paused.
	/// </summary>
	public void Pause()
	{
		paused = true;
	}
	
	/// <summary>
	/// Resume the currently paused clip. Will do nothing if the clip hasn't been paused.
	/// </summary>
	public void Resume()
	{
		paused = false;
	}
	
	void OnCompleteAnimation()
	{
		previousFrame = -1;
		if (animationCompleteDelegate != null)
			animationCompleteDelegate(this, clipId);
	}
	
	void SetFrame(int currFrame)
	{
		if (previousFrame != currFrame)
		{
			SwitchCollectionAndSprite( currentClip.frames[currFrame].spriteCollection, currentClip.frames[currFrame].spriteId );
			if (currentClip.frames[currFrame].triggerEvent)
			{
				if (animationEventDelegate != null)
					animationEventDelegate( this, currentClip, currentClip.frames[currFrame], currFrame );
			}
			previousFrame = currFrame;
		}
	}
	
	void FixedUpdate () //CHANGED Update with FixedUpdate mizardo
	{
#if UNITY_EDITOR
		// Don't play animations when not in play mode
		if (!Application.isPlaying)
			return;
#endif
		
		if (g_paused || paused)
			return;
		
		if (currentClip != null && currentClip.frames != null)
		{
			clipTime += Time.deltaTime * currentClip.fps * localTimeScale;
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int currFrame = (int)clipTime % currentClip.frames.Length;
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection)
			{
				int currFrame = (int)clipTime;
				if (currFrame >= currentClip.loopStart)
				{
					currFrame = currentClip.loopStart + ((currFrame - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart));
				}
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.PingPong)
			{
				int currFrame = (int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2);
				if (currFrame >= currentClip.frames.Length)
				{
					int i = currFrame - currentClip.frames.Length;
					currFrame = currentClip.frames.Length - 2 - i;
				}
				SetFrame(currFrame);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
			{
				int currFrame = (int)clipTime;
				if (currFrame >= currentClip.frames.Length)
				{
					currentClip = null;
					OnCompleteAnimation();
				}
				else
				{
					SetFrame(currFrame);
				}
				
			}
		}
	}
}
