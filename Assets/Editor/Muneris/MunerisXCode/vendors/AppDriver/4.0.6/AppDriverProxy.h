//
//  AppDriverProxy.h
//  AppDriverLibraryManagement
//
//  Created by qu.xiaoyi on 11/09/28.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "AppDriverRequest.h"
#import "AppDriverRewardItem.h"
#import "AppDriverRewardView.h"
#import "AppDriverAnalytics.h"

#import <StoreKit/StoreKit.h>

enum {
    DEFAULT_MODE = 255,
    PROMOTION_MODE = 1,
    GYRO_MODE = 2,
    ANALYTICS_MODE = 4,
    SETTING_MODE = 0
};
typedef NSInteger AppDriverServiceMode;

@class AppDriverRequestQueue;
@class AppDriverRequest;
@class AppDriverRewardItem;

@interface AppDriverProxy : NSObject{
@private
    NSString *_siteId;
    NSString *_siteKey;
    NSNumber *_refreshTime;
    NSString *_refreshURL;
    NSString *_identifier;
    NSMutableDictionary *_products;
    AppDriverRequestQueue *_requestQueue;
    BOOL _testMode;
    BOOL _developmentMode;
}

@property (nonatomic, retain) NSString *siteId;
@property (nonatomic, retain) NSString *siteKey;
@property (nonatomic, retain) NSNumber *refreshTime;
@property (nonatomic, retain) NSString *refreshURL;
@property (nonatomic, retain) NSString *identifier;
@property (nonatomic, retain) NSMutableDictionary *products;
@property (nonatomic, retain) AppDriverRequestQueue *requestQueue;
@property (nonatomic, retain) AppDriverAnalytics *analyticsInstant;
@property (nonatomic, assign) BOOL testMode;
@property (nonatomic, assign) BOOL developmentMode;

+ (AppDriverProxy*)sharedAppDriverProxy;
+ (void)setRefreshTime:(NSInteger)refreshTime AndRefreshURL:(NSString *)refreshURL;
+ (void)requestAppDriverWithSiteId:(NSString *)siteId siteKey:(NSString *)siteKey;
+ (void)requestAppDriverWithSiteId:(NSString *)siteId siteKey:(NSString *)siteKey mode:(AppDriverServiceMode)mode;
+ (void)requestAppDriverWithSiteId:(NSString *)siteId siteKey:(NSString *)siteKey mode:(AppDriverServiceMode)mode identifier:(NSString *)identifier;
+ (void)actionComplete:(NSString *)thanks;
+ (void)targetComplete:(NSString *)target;
+ (void)targetCancel:(NSString *)target;
+ (void)paymentComplete:(SKPaymentTransaction *)transaction;
+ (void)paymentComplete:(SKPaymentTransaction *)transaction WithProduct:(SKProduct *)product;
+ (void)addProduct:(NSArray *)products;
+ (SKProduct *)getProduct:(NSString *)productIdentifier;

+ (void)setTestMode:(BOOL)flag;

+ (AppDriverRequest *)getPromotionListPageRequest:(int)mediaId User:(NSString *)user Item:(AppDriverRewardItem *)item;
+ (AppDriverRequest *)getPromotionDetailPageRequest:(int)mediaId User:(NSString *)user Item:(AppDriverRewardItem *)item AtPromotionId:(int)promotionId;
+ (AppDriverRequest *)getPromotionStoreURLRequest:(int)mediaId User:(NSString *)user Item:(AppDriverRewardItem *)item AtPromotionId:(int)promotionId;
+ (AppDriverRequest *)getRewardStatusRequest:(NSInteger)mediaId User:(NSString *)identifier AtPromotionId:(NSInteger)promotionId;

+ (NSNumber *)getRewardStatus:(AppDriverRequest *)request;


+ (void)startSession;
+ (void)stopSession;
+ (void)logEvent:(NSString *)eventName;
+ (void)logEvent:(NSString *)eventName withParameters:(NSDictionary *)parameters;
+ (void)logError:(NSString *)errorID message:(NSString *)message exception:(NSException *)exception;

- (BOOL)initialSettingWithSiteId:(NSString *)siteId siteKey:(NSString *)siteKey identifier:(NSString *)identifier;

@end
