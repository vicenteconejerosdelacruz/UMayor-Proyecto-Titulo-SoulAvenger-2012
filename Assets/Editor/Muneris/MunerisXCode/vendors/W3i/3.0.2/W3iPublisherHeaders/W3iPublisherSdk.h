//
//  W3iPublisherSdk.h
//  W3iPublisherSdk
//
//  Created by Patrick Brennan on 10/6/11.
//  Copyright 2011 W3i. All rights reserved.

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "AfppUIThemeConfig.h"
#import "MaapBannerAdView.h"
#import "MaapInterstitialAdViewController.h"
#import "InAppPurchaseTrackRequest.h"

@protocol W3iPublisherDelegate;

/** Main class for W3iPublisherSDK

*/
@interface W3iPublisherSdk : NSObject

@property (nonatomic, copy) NSString *gameTitle;
@property (nonatomic, assign) BOOL showViolations;
@property (nonatomic, assign) BOOL showMessages;
@property (nonatomic, assign) id<AfppUIThemeConfig> themeConfig;
@property (nonatomic, retain) id<W3iPublisherDelegate> delegate;
@property (nonatomic, retain) UIPopoverController *offerWallPopover;
@property (nonatomic, assign) BOOL shouldShowRedeemAlert;
@property (nonatomic, assign) BOOL shouldGetUDID;


#pragma mark - Publisher API

/** This method provides access to the W3iPublisherSDK object.
    returns a singleton W3iPublisherSDK instance.
 */
+ (id)sharedInstance;

- (NSString *)getSDKVersion;

- (void)initiateWithAppId:(NSString *)appId andPublisherUserId:(NSString *) publisherUserId;

- (void)openOfferWallFromPresentingViewController:(UIViewController *)presentingViewController;

- (void)openOfferWallFromPopoverView:(UIView *)popoverView;

- (void)showFeaturedOffer;

- (void)redeemCurrency;

- (void)requestDataForITunesProductIds:(NSArray *)iTunesProductIdArray;

- (MaapBannerAdView *)bannerAdViewWithThemeID:(NSNumber *)themeID 
                                     delegate:(id<MaapBannerAdViewDelegate>)delegate 
                                        frame:(CGRect)frame;
- (MaapInterstitialAdViewController *)interstitialAdViewControllerWithThemeID:(NSNumber *)themeID 
                                                                     delegate:(id<MaapInterstitialAdViewControllerDelegate>)delegate;

- (InAppPurchaseTrackRequest *)trackInAppPurchaseWithTrackRecord:(InAppPurchaseTrackRecord *)trackRecord 
                                                        delegate:(id<InAppPurchaseTrackDelegate>)delegate; //if the delegate is about to be deallocated clear return value's delegate property

#pragma mark - W3i Advertiser API
// W3i Advertiser API

// call this to connect to W3i and inform that the app "appID" was run
// param: appID -- is the unique Identifier you receive from W3i
// call this in AppDidFinishLaunchingWithOptions
- (void) connectWithAppID:(NSString*)appID;

// call this to connect to W3i and inform that the app "actionID" was performed
// param: actionID -- is the unique Identifier for the action, that you receive from W3i
- (void)actionTakenWithActionID:(NSString*)actionID;

// call this when you no longer need the shared connector object
// usually in AppWillTerminate
- (void)close;

@end

#pragma mark - Publisher Delegates

@protocol W3iPublisherDelegate <NSObject>

@required

//** Called when the Offer Wall is successfully initialized.
- (void)w3iPublisherSdkDidInitiateWithIsOfferwallAvailable:(BOOL)isAvailable;

//** Called when there is an error trying to initialize the Offer Wall.

- (void)w3iPublisherSdkDidFailToInitiate: (NSError *) error;

//** Called when the currency redemption is successfull.
- (void)didRedeemWithBalances:(NSArray *)balances andReceiptId:(NSString *)receiptId;

//** Called when the currency redemption is unsuccessfull.
- (void)didRedeemWithError:(NSError *)error;

//** Called when publisher is about to display modally fullscreen instruction view for a chosen featured offer
- (UIViewController *)parentViewControllerForModallyPresentingInterstitialInstructionView:(UIViewController *)interstitialInstructionVC;

@optional
- (void)offerWallWillDisplay;
- (void)offerWallDidDisplay;
- (void)offerWallWillRedirectUserToAppStore;
- (void)offerWallWillDismiss;
- (void)offerWallDidDismiss;
- (void)featuredOfferNotAvailable;


@end
