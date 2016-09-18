//
//  Muneris.h
//  MunerisKit
//
//  Created by Jacky Yuk on 8/25/11.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//



#pragma mark 2.1
#import "MunerisMacro.h"
#import "MunerisConstant.h"
#import "MunerisConfiguration.h"
#import "MunerisTakeoverDelegate.h"
#import "MunerisOffersDelegate.h"
#import "MunerisInboxDelegate.h"
#import "MunerisBannerAdsDelegate.h"
#import "MunerisMessages.h"
#import "MunerisDeviceId.h"
#import "MunerisCallback.h"

@interface Muneris : NSObject {
  
@private
  BOOL _booted;
  
  NSDictionary* _parts;
  
}

+(void) setLogLevel:(MunerisLogLevel) logLevel;

+(void) bootWithDictionary:(NSDictionary*) jsonFile withApiKey:(NSString*) apiKey;

+(void) bootWithJsonFile:(NSString*) jsonFile withApiKey:(NSString*) apiKey;

+(void) bootWithJsonFile:(NSString*) jsonFile withApiKey:(NSString*) apiKey inBundle:(NSBundle*) bundle;

+(void) bootWithJsonString:(NSString*) json withApiKey:(NSString*) apiKey;

+(void) bootWithPlistFile:(NSString*) plistFile withApiKey:(NSString*) apiKey;

+(void) bootWithPlistFile:(NSString*) plistFile withApiKey:(NSString*) apiKey inBundle:(NSBundle*) bundle;

+(void) boot:(id<MunerisConfiguration>) configuration;

+(void) addCallback:(id<MunerisCallback>) delegate;

+(void) removeCallback:(id<MunerisCallback>) delegate;

+(void) purgeInstance;

+(BOOL) isReady;


#pragma mark - Muneris Device ID Methods

+(MunerisDeviceId*) getDeviceId;

#pragma mark - Muneris Banner Ads Methods

+(void) loadAdswithBannerSize:(MunerisBannerAdsSizes)size withZone:(NSString*) zone withDelegate:(id<MunerisBannerAdsDelegate>) delegate withUIViewController:(UIViewController*) viewController;

+(BOOL) hasBannerAds:(NSString*)zone;

#pragma mark - Muneris Takeover Methods

+(void) showTakeover:(NSString*)zone withViewController:(UIViewController*)viewController;

+(void) loadTakeover:(NSString*)zone withViewController:(UIViewController *)viewController withDelegate:(id<MunerisTakeoverDelegate>)delegate;

+(BOOL) hasTakeover:(NSString*)zone;


#pragma mark - Muneris Offers Methods

+(void) showOffers:(UIViewController*)viewController withDelegate:(id<MunerisOffersDelegate>)delegate;

+(BOOL) hasOffers;


#pragma mark - Muneris Analytics Methods

+(void) logEvent:(NSString *)eventName withParameters:(NSDictionary *)parameters;

+(void) logStartEvent:(NSString *)eventName withParameters:(NSDictionary *)parameters;

+(void) logEndEvent:(NSString *)eventName withParameters:(NSDictionary *)parameters;

#pragma mark Muneris WebView

+(BOOL) hasCustomerSupport;

+(void) showCustomerSupport:(UIViewController*) viewController;

+(BOOL) hasMoreApps;

+(void) showMoreApps:(UIViewController*) viewController;


#pragma mark - Muneris Inbox Methods

+(void) checkMessages:(NSArray*)type withDelegate:(id<MunerisInboxDelegate>)deleagte;


#pragma mark - Muneris Check App Version

+(void) checkAppVersion;


#pragma mark - Muneris Pay Per Action

+(void) actionComplete:(NSString*)actionKey;


#pragma mark - UIApplication Delegates

+ (void)applicationDidFinishLaunching:(UIApplication *)application;
+ (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;

+ (void)applicationDidBecomeActive:(UIApplication *)application;
+ (void)applicationWillResignActive:(UIApplication *)application;
+ (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url;
+ (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

+ (void)applicationDidReceiveMemoryWarning:(UIApplication *)application;
+ (void)applicationWillTerminate:(UIApplication *)application;
+ (void)applicationSignificantTimeChange:(UIApplication *)application;

+ (void)application:(UIApplication *)application willChangeStatusBarOrientation:(UIInterfaceOrientation)newStatusBarOrientation duration:(NSTimeInterval)duration;
+ (void)application:(UIApplication *)application didChangeStatusBarOrientation:(UIInterfaceOrientation)oldStatusBarOrientation;

+ (void)application:(UIApplication *)application willChangeStatusBarFrame:(CGRect)newStatusBarFrame;
+ (void)application:(UIApplication *)application didChangeStatusBarFrame:(CGRect)oldStatusBarFrame;

+ (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken;
+ (void)application:(UIApplication *)application didFailToRegisterForRemoteNotificationsWithError:(NSError *)error;

+ (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo;
+ (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification;

+ (void)applicationDidEnterBackground:(UIApplication *)application;
+ (void)applicationWillEnterForeground:(UIApplication *)application;

+ (void)applicationProtectedDataWillBecomeUnavailable:(UIApplication *)application;
+ (void)applicationProtectedDataDidBecomeAvailable:(UIApplication *)application;


@end