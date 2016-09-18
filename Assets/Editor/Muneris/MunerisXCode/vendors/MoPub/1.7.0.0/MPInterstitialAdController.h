//
//  MPInterstitialAdController.h
//  MoPub
//
//  Created by Andrew He on 2/2/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MPAdView.h"
#import "MPBaseInterstitialAdapter.h"

enum
{
	InterstitialCloseButtonStyleAlwaysVisible,
	InterstitialCloseButtonStyleAlwaysHidden,
	InterstitialCloseButtonStyleAdControlled
};
typedef NSUInteger InterstitialCloseButtonStyle;

enum 
{
	InterstitialOrientationTypePortrait,
	InterstitialOrientationTypeLandscape,
	InterstitialOrientationTypeBoth
};
typedef NSUInteger InterstitialOrientationType;

@protocol MPInterstitialAdControllerDelegate;

@interface MPInterstitialAdController : UIViewController <MPAdViewDelegate, MPBaseInterstitialAdapterDelegate>
{
	// Previous state of the status bar, before the interstitial appears.
	BOOL _statusBarWasHidden;
	
	// Previous state of the nav bar, before the interstitial appears.
	BOOL _navigationBarWasHidden;
	
	// Whether the interstitial is fully loaded.
	BOOL _ready;
	
	// Underlying ad view used for the interstitial.
	MPInterstitialAdView *_adView;
    
    // Delegate for this interstitial.
    id<MPInterstitialAdControllerDelegate> _delegate;
    
    // The view controller being used to present the interstitial.
    UIViewController *_rootViewController;
	
	// The ad unit ID.
	NSString *_adUnitId;
	
	// Determines how/when the interstitial should display a native close button.
	InterstitialCloseButtonStyle _closeButtonStyle;

	// Whether the ad content has requested that a native close button be shown.
	BOOL _adWantsNativeCloseButton;
	
	// Determines the allowed orientations for the interstitial.
	InterstitialOrientationType _orientationType;
	
	// Button used to dismiss the interstitial.
	UIButton *_closeButton;
	
	MPBaseInterstitialAdapter *_currentAdapter;
    
    // Whether the interstitial is currently being presented.
    BOOL _isOnModalViewControllerStack;
    
    // Deprecated: reference to the view controller that is presenting this interstitial.
	UIViewController<MPInterstitialAdControllerDelegate> *_parent;
}

@property (nonatomic, readonly, assign) BOOL ready;
@property (nonatomic, assign) id<MPInterstitialAdControllerDelegate> delegate;
@property (nonatomic, copy) NSString *adUnitId;
@property (nonatomic, copy) NSString *keywords;
@property (nonatomic, copy) CLLocation *location;
@property (nonatomic, assign) BOOL locationEnabled;
@property (nonatomic, assign) NSUInteger locationPrecision;
@property (nonatomic, assign) BOOL adWantsNativeCloseButton;

// Deprecated properties.
@property (nonatomic, assign) UIViewController<MPInterstitialAdControllerDelegate> *parent
    MOPUB_DEPRECATED;

/*
 * A shared pool of interstitial ads.
 */
+ (NSMutableArray *)sharedInterstitialAdControllers;

/*
 * Gets an interstitial for the given ad unit ID. Once created, an interstitial will stay around
 * so that you can retrieve it later.
 */
+ (MPInterstitialAdController *)interstitialAdControllerForAdUnitId:(NSString *)ID;

/*
 * Removes an interstitial from the shared pool.
 */
+ (void)removeSharedInterstitialAdController:(MPInterstitialAdController *)controller;

/*
 * Begin loading the content for the interstitial ad. You may implement the -interstitialDidLoadAd
 * and -interstitialDidFailToLoadAd delegate methods, so that you can decide when to show the ad.
 * This method does not automatically retry if it fails.
 */
- (void)loadAd;

/*
 * Display the interstitial modally from the specified view controller.
 */
- (void)showFromViewController:(UIViewController *)controller;

/*
 * Returns the result of -locationDescriptionPair on the embedded ad view.
 */
- (NSArray *)locationDescriptionPair;

/*
 * Signals to the ad view that a custom event has caused ad content to load
 * successfully. You must call this method if you implement custom events.
 */
- (void)customEventDidLoadAd;

/*
 * Signals to the ad view that a custom event has resulted in a failed load.
 * You must call this method if you implement custom events.
 */
- (void)customEventDidFailToLoadAd;

/*
 * Signals to the ad view that a user has tapped on a custom-event-triggered ad.
 * You must call this method if you implement custom events, for proper click tracking.
 */
- (void)customEventActionWillBegin;

// Deprecated methods.

/*
 * DEPRECATED: Display the interstitial modally. Use -showFromViewController: instead.
 * This method will only work correctly if the interstitial's parent property is set.
 */
- (void)show MOPUB_DEPRECATED;

@end

@protocol MPInterstitialAdControllerDelegate <NSObject>

@optional

/*
 * These callbacks notify you when the interstitial (un)successfully loads its ad content. You may
 * implement these if you want to prefetch interstitial ads.
 */
- (void)interstitialDidLoadAd:(MPInterstitialAdController *)interstitial;
- (void)interstitialDidFailToLoadAd:(MPInterstitialAdController *)interstitial;

/*
 * This callback notifies you that the interstitial is about to appear. This is a good time to
 * handle potential app interruptions (e.g. pause a game).
 */
- (void)interstitialWillAppear:(MPInterstitialAdController *)interstitial;
- (void)interstitialDidAppear:(MPInterstitialAdController *)interstitial;
- (void)interstitialWillDisappear:(MPInterstitialAdController *)interstitial;
- (void)interstitialDidDisappear:(MPInterstitialAdController *)interstitial;

/*
 * Interstitial ads from certain networks (e.g. iAd) may expire their content at any time, 
 * regardless of whether the content is currently on-screen. This callback notifies you when the
 * currently-loaded interstitial has expired and is no longer eligible for display. If the ad
 * was on-screen when it expired, you can expect that the ad will already have been dismissed 
 * by the time this callback was fired. Your implementation may include a call to -loadAd to fetch a
 * new ad, if desired.
 */
- (void)interstitialDidExpire:(MPInterstitialAdController *)interstitial;

/*
 * DEPRECATED: This callback notifies you to dismiss the interstitial, and allows you to implement
 * any pre-dismissal behavior (e.g. unpausing a game). This method is being deprecated as it is no
 * longer necessary to dismiss an interstitial manually (i.e. via calling
 * -dismissModalViewControllerAnimated:).
 *
 * Any pre-dismissal behavior should be implemented using -interstitialWillDisappear: or
 * -interstitialDidDisappear: instead.
 */
- (void)dismissInterstitial:(MPInterstitialAdController *)interstitial MOPUB_DEPRECATED;

/*
 * DEPRECATED: These methods were previously inherited from MPAdViewDelegate. They should no longer
 * be used.
 */
- (UIViewController *)viewControllerForPresentingModalView MOPUB_DEPRECATED;
- (void)adViewDidFailToLoadAd:(MPAdView *)view MOPUB_DEPRECATED;
- (void)adViewDidLoadAd:(MPAdView *)view MOPUB_DEPRECATED;
- (void)willPresentModalViewForAd:(MPAdView *)view MOPUB_DEPRECATED;
- (void)didDismissModalViewForAd:(MPAdView *)view MOPUB_DEPRECATED;
- (void)adView:(MPAdView *)view didReceiveResponseParams:(NSDictionary *)params MOPUB_DEPRECATED;
- (void)adViewShouldClose:(MPAdView *)view MOPUB_DEPRECATED;

@end

