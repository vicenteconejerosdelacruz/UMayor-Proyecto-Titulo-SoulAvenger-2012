//
//  MMAdView.h
//
//  Copyright 2010 Millennial Media Inc. All rights reserved.
//


#import <UIKit/UIKit.h>
#import <CoreLocation/CoreLocation.h>

@protocol MMAdDelegate;

typedef enum AdType {
	MMBannerAdTop = 1,  //320x53
	MMBannerAdBottom = 2,
	MMBannerAdRectangle = 3,
	MMFullScreenAdLaunch = 4,
	MMFullScreenAdTransition = 5 //interstitial
} MMAdType;

typedef enum LogLevel {
    LOG_LEVEL_OFF   = 0,
    LOG_LEVEL_INFO  = 1 << 0,
    LOG_LEVEL_DEBUG = 1 << 1,
    LOG_LEVEL_ERROR = 1 << 2,
    LOG_LEVEL_FATAL = 1 << 3
} MMLogLevel;

@interface MMAdView : UIView {
	id<MMAdDelegate> delegate;
	BOOL refreshTimerEnabled;
	NSTimeInterval adRequestTimeoutInterval;
	
}

@property(nonatomic,assign) id<MMAdDelegate> delegate;
@property(nonatomic) BOOL refreshTimerEnabled;
@property(nonatomic) NSTimeInterval adRequestTimeoutInterval;
@property(nonatomic) UIInterfaceOrientation overlayOrientation;
@property(nonatomic) CGRect overlayFrame;
@property(nonatomic,assign) UIViewController *rootViewController;

/**
 * Creates and returns an autoreleased MMAdView
 */
+ (MMAdView *) adWithFrame:(CGRect)aFrame type:(MMAdType) type apid:(NSString *) apid delegate: (id<MMAdDelegate>) aDelegate loadAd: (BOOL) loadAd startTimer: (BOOL) startTimer;
	

/**
 * Creates and returns an autoreleased interstitial MMAdView with no timer running.
 */
+ (MMAdView *) interstitialWithType:(MMAdType) type apid: (NSString *) apid delegate: (id<MMAdDelegate>)aDelegate loadAd: (BOOL) loadAd;


/**
 * Use this method to start the conversion tracker.
 */
+ (void) startSynchronousConversionTrackerWithGoalId: (NSString *) goalid;

/**
 * Returns a NSString of the version of this SDK.
 * ex: 4.0.8
 */
+ (NSString *) version;

/**
 * Updates the location used for ad requests
 */
+ (void) updateLocation: (CLLocation *) currentLocation;


/**
 * Sets log levels
 */
+ (void)setLogLevel:(MMLogLevel)level;

/**
 * Will load a new ad. 
 * If MMAdView was created with loadAdImmediately set to NO, refereshAd must be called everytime to get a new ad.
 */

- (void) refreshAd;

/**
 * Requests an ad and caches it for display at a later time.
 * Returns YES is a fetch was made, NO if was unable to or an ad had already been fetched.
 */
- (BOOL) fetchAdToCache;

/**
 * Checks for a cached ad 
 * Returns YES if an ad is available or display, otherwise returns NO
 */
- (BOOL) checkForCachedAd;

/**
 * Displays a ad cached using the fetchAdToCache method
 * Returns YES if an ad will display, otherwise returns NO.
 */
- (BOOL) displayCachedAd;
	

@end



@protocol MMAdDelegate<NSObject>
@optional
	
- (NSDictionary *)requestData;

// Set the timer duration for the rotation of ads in seconds. Default: 60
- (NSInteger)adRefreshDuration;
	
/**
 * Use this method to disable the accelerometer. Default: YES
 */	
- (BOOL)accelerometerEnabled;
	
/**
 * If the following methods are implemented, the delegate will be notified when
 * an ad request succeeds or fails. An ad request is considered to have failed
 * in any situation where no ad is received.
 */

- (void)adRequestSucceeded:(MMAdView *) adView;
- (void)adRequestFailed:(MMAdView *) adView;
- (void)adDidRefresh:(MMAdView *) adView;
- (void)adWasTapped:(MMAdView *) adView;

- (void)adRequestIsCaching:(MMAdView *) adView;
- (void)adRequestFinishedCaching:(MMAdView *) adView successful: (BOOL) didSucceed;

- (void)applicationWillTerminateFromAd;


- (void)adModalWillAppear;
- (void)adModalDidAppear;
- (void)adModalWasDismissed;


@end