//
//  SmartMadAdView.h
//  view for SmartMad 
//
//  Created by MadClient on 7/25/12.
//  Copyright 2011 Madhouse Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "SmartMadDelegate.h"

@class SmartMadAdView;

// SmartMad event delegate protocol
@protocol SmartMadAdEventDelegate<NSObject>

- (void)adEvent:(SmartMadAdView*)adview  adEventCode:(AdEventCodeType)eventCode;
- (void)adFullScreenStatus:(BOOL)isFullScreen;

@end

// SmartMad ad view
@interface SmartMadAdView : UIView {
	
@public
	
	id<SmartMadAdEventDelegate>  _adEventDelegate;
}

@property(nonatomic,assign)id<SmartMadAdEventDelegate>  _adEventDelegate;

// interface for use
// set some option parameters
+(void)setApplicationId:(NSString *)applicationId;
+(void)setKeyWord:(NSString*)aKeyWord;
+(void)setUserGender:(AdUserGen)aGender;
+(void)setUserAge:(NSInteger)age;
+(void)setBirthDay:(NSString*)aBirthDay;
+(void)setFavorite:(NSString*)aFavorite;
+(void)setCity:(NSString*)aCityName;
+(void)setPostalCode:(NSString*)aPostalCode;
+(void)setWork:(NSString*)aWorkType;
+(void)skipCurrentFullscreenAd;

// construction
-(SmartMadAdView*)initRequestAdWithDelegate:(id<SmartMadAdViewDelegate>)adelegate;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID  compileMode:(AdCompileMode)compileMode;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID aInterval:(NSTimeInterval)aInterval   compileMode:(AdCompileMode)compileMode;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID aInterval:(NSTimeInterval)aInterval   adMeasure:(AdMeasureType)adMeasure  compileMode:(AdCompileMode)compileMode;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID aInterval:(NSTimeInterval)aInterval
									adMeasure:(AdMeasureType)adMeasure adBannerAnimation:(AdBannerTransitionAnimationType)adBannerAnimation compileMode:(AdCompileMode)compileMode;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID aInterval:(NSTimeInterval)aInterval
                                    adMeasure:(AdMeasureType)adMeasure withOrigin:(CGPoint)originPoint compileMode:(AdCompileMode)compileMode;

-(SmartMadAdView*)initRequestAdWithParameters:(NSString*)posID aInterval:(NSTimeInterval)aInterval
                                    adMeasure:(AdMeasureType)adMeasure withOrigin:(CGPoint)originPoint adBannerAnimation:(AdBannerTransitionAnimationType)adBannerAnimation compileMode:(AdCompileMode)compileMode;

// set event delegate
-(void)setEventDelegate:(id<SmartMadAdEventDelegate>)aEventDelegate;

@end
