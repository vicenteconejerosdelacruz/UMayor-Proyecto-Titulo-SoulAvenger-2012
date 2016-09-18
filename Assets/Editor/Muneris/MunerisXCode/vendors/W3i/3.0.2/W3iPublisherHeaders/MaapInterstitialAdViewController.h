//
//  MaapInterstitialAdViewController.h
//  W3iPublisherSdk
//
//  Created by Bozhidar Mihaylov on 12/22/11.
//  Copyright (c) 2011 MentorMate. All rights reserved.
//

#import <UIKit/UIKit.h>

@protocol MaapInterstitialAdViewControllerDelegate;
@interface MaapInterstitialAdViewController : UIViewController

@property (nonatomic, assign) id<MaapInterstitialAdViewControllerDelegate> delegate;
@property (nonatomic, copy) NSNumber *themeID;

@property (nonatomic, readonly, getter = isAdLoading) BOOL adLoading;
@property (nonatomic, readonly, getter = isAdLoaded) BOOL adLoaded;

//this can be set as an alternative to implementing a delegate
//(Modal view presents on and dismisses from this property if not nil)
@property (nonatomic, assign) UIViewController *parentController;

//the modal view can be set to auto dismiss after a set amount of time
@property (nonatomic, assign) NSTimeInterval autoDismissTime;

//the modal view can be set to landscape mode (defaults to portrait)
@property (nonatomic, assign) BOOL landscapeOrientation;

//whether view controller to handle status bar when visible (hide on present and show on dismiss).
@property (nonatomic, assign) BOOL handleStatusBar;

//animation for hiding status bar (default to no animation)
@property (nonatomic, assign) UIStatusBarAnimation statusBarHideAnimation;

//the close round button exposed for customization
@property (nonatomic, readonly) UIButton *closeButton;

+ (id)interstitialAdViewControllerWithThemeID:(NSNumber *)themeID 
                                     delegate:(id<MaapInterstitialAdViewControllerDelegate>)delegate;
- (id)initWithThemeID:(NSNumber *)themeID 
             delegate:(id<MaapInterstitialAdViewControllerDelegate>)delegate;

- (void)reloadAdContent;

//popover style for ipad
- (void)presentPopoverFromRect:(CGRect)rect 
                        inView:(UIView *)view 
      permittedArrowDirections:(UIPopoverArrowDirection)arrowDirections;

- (void)presentPopoverFromBarButtonItem:(UIBarButtonItem *)item 
               permittedArrowDirections:(UIPopoverArrowDirection)arrowDirections;

@end

@protocol MaapInterstitialAdViewControllerDelegate <NSObject>

@required
- (void)didLoadContentForInterstitialAdViewController:(MaapInterstitialAdViewController *)adView;
- (void)noAdContentForInterstitialAdViewController:(MaapInterstitialAdViewController *)adView;
- (void)interstitialAdViewController:(MaapInterstitialAdViewController *)adView 
                  didFailedWithError:(NSError *)error;

@optional
- (BOOL)interstitialAdViewController:(MaapInterstitialAdViewController *)adView 
    shouldLeaveApplicationOpeningURL:(NSURL *)url;
// called when view allows its delegate to dismiss it when it's done
- (void)dismissActionForInterstitialAdViewController:(MaapInterstitialAdViewController *)adView;
// called when modal or popover view disappear from screen
- (void)didDismissForInterstitialAdViewController:(MaapInterstitialAdViewController *)adView;

@end