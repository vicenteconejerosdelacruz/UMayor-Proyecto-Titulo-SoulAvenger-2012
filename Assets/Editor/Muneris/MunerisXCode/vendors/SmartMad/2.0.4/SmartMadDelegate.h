//
//  SmartMadDelegate.h
//  delegate for SmartMad
//
//  Created by MadClient on 7/25/12.
//  Copyright 2011 Madhouse Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

// ad measure
typedef enum
{
    AD_MEASURE_DEFAULT=0,
	TABLET_AD_MEASURE_300X250=7,
	TABLET_AD_MEASURE_468X60=8,
	TABLET_AD_MEASURE_728X90=9,
	
} AdMeasureType; 

// banner transform animation
typedef enum
{
	BANNER_ANIMATION_TYPE_NONE = 0,
	BANNER_ANIMATION_TYPE_RANDOM = 1,
	BANNER_ANIMATION_TYPE_FADEINOUT = 2,
	BANNER_ANIMATION_TYPE_FLIPFROMLEFT = 3,
	BANNER_ANIMATION_TYPE_FLIPFROMRIGHT = 4,
	BANNER_ANIMATION_TYPE_CURLUP = 5,
	BANNER_ANIMATION_TYPE_CURLDOWN = 6,
	BANNER_ANIMATION_TYPE_SLIDEFROMLEFT = 7,
	BANNER_ANIMATION_TYPE_SLIDEFROMRIGHT = 8,
	
} AdBannerTransitionAnimationType;

// user gender
typedef enum
{
	UFemale=1,
	UMale=2   
	
} AdUserGen;

// SmartMad release mode
typedef enum
{
	AdRelease=0,   
	AdDebug=1
	
} AdCompileMode;

// ad evnet code
typedef enum
{
	EVENT_NEWAD = 1,
	EVENT_INVALIDAD = 2
	
} AdEventCodeType;


@protocol SmartMadAdViewDelegate<NSObject>

#pragma mark required
@required
// required parameters settings
-(NSString*)adPositionId;

@optional
// optional parameters settings
-(NSTimeInterval)adInterval;
-(AdMeasureType)adMeasure;
-(AdBannerTransitionAnimationType)adBannerAnimation;
-(AdCompileMode)compileMode;

@end

