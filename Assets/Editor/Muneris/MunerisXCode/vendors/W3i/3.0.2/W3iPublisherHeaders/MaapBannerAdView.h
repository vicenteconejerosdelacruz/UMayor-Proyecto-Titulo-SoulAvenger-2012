//
//  MaapBannerAdView.h
//  W3iPublisherSdk
//
//  Created by Bozhidar Mihaylov on 12/19/11.
//  Copyright (c) 2011 MentorMate. All rights reserved.
//

#import <UIKit/UIKit.h>


@protocol MaapBannerAdViewDelegate;
@interface MaapBannerAdView : UIView

@property (nonatomic, assign) id<MaapBannerAdViewDelegate> delegate;
@property (nonatomic, copy) NSNumber *themeID;

@property (nonatomic, readonly, getter = isAdLoading) BOOL adLoading;
@property (nonatomic, readonly, getter = isAdLoaded) BOOL adLoaded;

+ (CGFloat)defaultBannerHeight;

+ (id)bannerAdViewWithThemeID:(NSNumber *)themeID 
                     delegate:(id<MaapBannerAdViewDelegate>)delegate 
                        frame:(CGRect)frame;
- (id)initWithThemeID:(NSNumber *)themeID 
             delegate:(id<MaapBannerAdViewDelegate>)delegate 
                frame:(CGRect)frame;

- (void)reloadAdContent;


@end

@protocol MaapBannerAdViewDelegate <NSObject>

@required
- (void)didLoadContentForBannerAdView:(MaapBannerAdView *)adView;
- (void)noAdContentForBannerAdView:(MaapBannerAdView *)adView;
- (void)bannerAdView:(MaapBannerAdView *)adView didFailedWithError:(NSError *)error;

@optional
- (BOOL)bannerAdView:(MaapBannerAdView *)adView shouldLeaveApplicationOpeningURL:(NSURL *)url;
- (void)dismissActionForBannerAdView:(MaapBannerAdView *)adView;

@end