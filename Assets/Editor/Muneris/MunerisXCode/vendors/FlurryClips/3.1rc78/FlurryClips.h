//
//  FlurryClips.h
//  Flurry iPhone Clips Agent
//
//  Copyright 2011 Flurry, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@class FlurryVideoOffer;


@interface FlurryClips : NSObject
{}

/*
 enable Video Ads. default is NO. AppCircle must be enabled.
 */
+ (void)setVideoAdsEnabled:(BOOL)value;

/*
 returns whether there is a video ad available to show
 */
+ (BOOL)videoAdIsAvailable:(NSString *)hook;

/*
 returns the number of Video Ads available for the hook
 */
+ (int)getVideoAdCount:(NSString*)hook;

/*
 rotates from current ad to the next for the hook
 */
+ (void)rotateVideoOffer:(NSString*)hook;

/* 
 obtain a look at meta data for next video ad to customize cookies or offers based on that value 
 */
+ (BOOL)peekVideoOffer:(NSString*)hookName withFlurryVideoOfferContainer:(FlurryVideoOffer*)flurryVideoOffer;

/* 
 open the fullscreen Video Ad takeover window
 */
+ (void)openVideoTakeover:(NSString *)hook orientation:orientationString rewardImage:(UIImage *)image rewardMessage:(NSString *)message userCookies:(NSDictionary*)userCookies;

/* 
 open the fullscreen Video Ad takeover window with option to autoPlay (skip first screen)
 */
+ (void)openVideoTakeover:(NSString *)hook orientation:orientationString rewardImage:(UIImage *)image rewardMessage:(NSString *)message userCookies:(NSDictionary*)userCookies autoPlay:(BOOL)autoPlay;

/*
 refer to FlurryAdDelegate.h for delegate details
 */
+ (void)setVideoDelegate:(id)delegate;

@end
