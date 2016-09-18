
#import <UIKit/UIKit.h>

#import "Muneris.h"
#import "MunerisIap.h"
#import "MunerisPurchaseCallback.h"


#import "CJSONDeserializer.h"
#import "CJSONSerializer.h"

// CONSTANTS ///////////////////////////////////

static const char* MUNERIS_OBJECT = "Muneris";

static const int ALIGNMENT_TOP = 0;
static const int ALIGNMENT_BOTTOM = 1;

UIViewController* UnityGetGLViewController();

// STATE ///////////////////////////////////////

static NSString* apiKey = nil;
static NSString* apiParams = @"{\"via\":{\"name\":\"unity\", \"ver\":\"2.2.1\", \"platform\":\"unity\"}}";
static NSDictionary* config = nil;

static NSArray* alertOptions = nil;
static MunerisAdsView* adView = nil;
static UIView* adHolder = nil;
static int bannerAlignment = 0;

static UIView* takeoverView = nil;

static bool adsPending = false;

extern void SetInputEnabled(bool which);

// CONFIGURATION DELEGATE //////////////////////

@interface ConfigurationDelegate : NSObject<MunerisConfiguration>
{
    
}

-(NSDictionary*) configuration;
-(NSString*) apiKey;
-(NSString*) extraApiParams;

@end

@implementation ConfigurationDelegate

-(NSDictionary*) configuration
{
    return [config retain];
}

-(NSString*) apiKey
{
    return [apiKey retain];
}

-(NSString*) extraApiParams
{
    return [apiParams retain];
}

@end

// NOTIFICATION DELEGATE ///////////////////////

@interface NotificationDelegate : NSObject
{
    
}

-(void) applicationDidBecomeActive:(NSNotification*)notification;
-(void) applicationWillResignActive:(NSNotification*)notification;
-(void) applicationDidEnterBackground:(NSNotification*)notification;
-(void) applicationWillEnterForeground:(NSNotification*)notification;
-(void) applicationWillTerminate:(NSNotification*)notification;

@end

@implementation NotificationDelegate

-(void) applicationDidBecomeActive:(NSNotification*)notification
{
    [Muneris applicationDidBecomeActive:[UIApplication sharedApplication]];
}

-(void) applicationWillResignActive:(NSNotification*)notification
{
    [Muneris applicationWillResignActive:[UIApplication sharedApplication]];    
}

-(void) applicationDidEnterBackground:(NSNotification*)notification
{
    [Muneris applicationDidEnterBackground:[UIApplication sharedApplication]];   
}

-(void) applicationWillEnterForeground:(NSNotification*)notification
{
    [Muneris applicationWillEnterForeground:[UIApplication sharedApplication]];      
}

-(void) applicationWillTerminate:(NSNotification*)notification
{
    [Muneris applicationWillTerminate:[UIApplication sharedApplication]];          
}

@end

static NotificationDelegate* notificationDelegate = nil;

// ALERT DELEGATE //////////////////////////////

@interface AlertDelegate : UIViewController<UIAlertViewDelegate>
{
    // ...
}

-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex;
-(void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex;

@end

@implementation AlertDelegate

-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    /*NSLog(@"Button pressed: %d", buttonIndex);
    UnitySendMessage(MUNERIS_OBJECT, "onAlertClosed", [[alertOptions objectAtIndex:buttonIndex] cStringUsingEncoding:NSUTF8StringEncoding]);
    
    [alertOptions release];
    alertOptions = nil;*/
}

-(void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex
{
    NSLog(@"Button pressed: %d", buttonIndex);
    UnitySendMessage(MUNERIS_OBJECT, "onAlertClosed", [[alertOptions objectAtIndex:buttonIndex] cStringUsingEncoding:NSUTF8StringEncoding]);
    
    [alertOptions release];
    alertOptions = nil;
}

@end

// PURCHASE DELEGATE ///////////////////////////

@interface PurchaseDelegate : NSObject<MunerisPurchaseCallback>
{
    // ...
}

-(void) purchaseDidComplete:(NSString*) skuNo;
-(void) purchaseDidCancel:(NSString *)skuNo withUserInfo:(NSDictionary *)dictionary;
-(void) purchaseDidFail:(NSString *)skuNo withUserInfo:(NSDictionary *)dictionary;

@end

@implementation PurchaseDelegate



-(void) purchaseDidComplete:(NSString *)skuNo;
{
    UnitySendMessage(MUNERIS_OBJECT, "IapSuccess", [skuNo cStringUsingEncoding:NSUTF8StringEncoding]);
}

-(void) purchaseDidCancel:(NSString *)skuNo withUserInfo:(NSDictionary *)dictionary;
{
    UnitySendMessage(MUNERIS_OBJECT, "IapCancelled", [skuNo cStringUsingEncoding:NSUTF8StringEncoding]);
}

-(void) purchaseDidFail:(NSString *)skuNo withUserInfo:(NSDictionary *)dictionary;
{
    UnitySendMessage(MUNERIS_OBJECT, "IapFailed", [skuNo cStringUsingEncoding:NSUTF8StringEncoding]);    
}

@end

// MESSAGE DELEGATE ////////////////////////////

static NSString* serializeMessages(MunerisMessages* messages)
{
    NSMutableArray* res = [NSMutableArray array];
    
    for (NSDictionary* msg in messages.messages)
    {
        switch ([[msg objectForKey:@"MunerisMessageType"] intValue])
        {
            case MunerisCreditsMessage:
            {  
                NSMutableDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:[msg objectForKey:@"subj"] forKey:@"subj"];
                [dict setValue:[[msg objectForKey:@"body"] objectForKey:@"credits"] forKey:@"credits"];
                [dict setValue:@"" forKey:@"body"];
                [dict setValue:@"c" forKey:@"type"];
                
                [res addObject:dict];
            }
            break;
                
            case MunerisTextMessage:
            {
                NSMutableDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:[msg objectForKey:@"subj"] forKey:@"subj"];
                [dict setValue:[NSNumber numberWithInt:0] forKey:@"credits"];
                [dict setValue:[[msg objectForKey:@"body"] objectForKey:@"text"] forKey:@"body"];
                [dict setValue:@"t" forKey:@"type"];
                
                [res addObject:dict];
            }
            break;
        }
    }
    
    NSError* err = nil;
    return [[NSString alloc] initWithData:[[CJSONSerializer serializer] serializeArray:res error:&err] encoding:NSUTF8StringEncoding];
}

@interface MessageDelegate : NSObject<MunerisInboxDelegate>
{
    // ...
}

-(void) didRecievedMessage:(MunerisMessages*) msgs;

@end

@implementation MessageDelegate

-(void) didRecievedMessage:(MunerisMessages*) msgs
{
    UnitySendMessage(MUNERIS_OBJECT, "onMessagesReceived", [serializeMessages(msgs) cStringUsingEncoding:NSUTF8StringEncoding]);    
}

@end

// BANNER DELEGATE /////////////////////////////

static void updateBannerAlignment()
{
    [adView removeFromSuperview];
    
    CGRect bounds = [[UIScreen mainScreen] bounds];
    BOOL landscape = UIInterfaceOrientationIsLandscape([UIApplication sharedApplication].statusBarOrientation);    
    
    const float length = (landscape ? bounds.size.width : bounds.size.height);
    const float width = (landscape ? bounds.size.height : bounds.size.width);
    
    const float y = (bannerAlignment == ALIGNMENT_BOTTOM) ? length - adView.frame.size.height : 0.0f;
    
    [adView setFrame:CGRectMake((width/2.0f) - (adView.frame.size.width / 2.0f),
                                     y,
                                     adView.frame.size.width,
                                     adView.frame.size.height)];
    
    [adHolder setFrame: adView.frame];
    [adHolder addSubview:adView];
}

@interface BannerDelegate : NSObject<MunerisBannerAdsDelegate>
{
    // ...
}

-(void) didReceiveAds:(MunerisAdsView*)view;
-(void) didFailToReceiveAds:(NSError*)error;

@end

@implementation BannerDelegate

-(void) didReceiveAds:(MunerisAdsView*)view
{  
    if ( adsPending == false )
        return;
    
    adsPending = false;
    
    if ( adHolder == nil )
    {
        UnitySendMessage(MUNERIS_OBJECT, "onBannerFailed", "");
        return;
    }
    
    if ( adView != nil )
    {
        [adHolder setFrame:CGRectMake(0,0,0,0)];
        [adView removeFromSuperview];
        [adView release];
    }
    
    adView = [view retain];
    updateBannerAlignment();
    
    UnitySendMessage(MUNERIS_OBJECT, "onBannerLoaded", "");
}

-(void) didFailToReceiveAds:(NSError*)error
{
    adsPending = false;
    UnitySendMessage(MUNERIS_OBJECT, "onBannerFailed", "");
}

@end

// TAKEOVER DELEGATE /////////////////////////////

@interface TakeoverDelegate : NSObject<MunerisTakeoverDelegate>
{
    // ...
}

-(BOOL) shouldShowTakeover:(NSDictionary*) takeoverInfo;
-(void) didFailToLoadTakeover:(NSDictionary*) takeoverInfo;
-(void) didFinishedLoadingTakeover:(NSDictionary*) takeoverInfo;
-(void) didDismissTakeover;

@end

@interface UIView (UserInteractionFeatures)
-(void)setRecursiveUserInteraction:(BOOL)value;
@end

@implementation UIView(UserInteractionFeatures)
-(void)setRecursiveUserInteraction:(BOOL)value{
    NSLog(@"Setting interaction enabled...");
    self.userInteractionEnabled =   value;
    for (UIView *view in [self subviews]) {
        [view setRecursiveUserInteraction:value];
    }
}
@end

@implementation TakeoverDelegate

-(BOOL) shouldShowTakeover:(NSDictionary*) takeoverInfo
{
    return TRUE;
}

-(void) didFailToLoadTakeover:(NSDictionary*) takeoverInfo
{
    NSLog(@"Failed to load takeover");
    UnitySendMessage(MUNERIS_OBJECT, "didFailedToLoadTakeover", "");
}

-(void) didFinishedLoadingTakeover:(NSDictionary*) takeoverInfo
{
    NSLog(@"Finished loading takeover");
    UnitySendMessage(MUNERIS_OBJECT, "didFinishedLoadingTakeover", "");  
}

-(void) didDismissTakeover
{
    UnitySendMessage(MUNERIS_OBJECT, "onDismissTakeover", ""); 
    
    if ( takeoverView != nil )
    {
        [takeoverView removeFromSuperview];
        [takeoverView release];
        takeoverView = nil;
    }
}

@end

// OFFERS DELEGATE ///////////////////////////////

@interface OfferDelegate : NSObject<MunerisOffersDelegate>
{
    // ...
}

-(BOOL) shouldShowTakeover:(NSDictionary*) takeoverInfo;
-(void) didFailToLoadTakeover:(NSDictionary*) takeoverInfo;
-(void) didFinishedLoadingTakeover:(NSDictionary*) takeoverInfo;
-(void) didDismissTakeover;

@end

@implementation OfferDelegate

-(BOOL) shouldShowTakeover:(NSDictionary*) takeoverInfo
{
    return TRUE;
}

-(void) didFailToLoadTakeover:(NSDictionary*) takeoverInfo
{
    NSLog(@"Failed to load takeover for offer");
    
    //UIViewController* contr = UnityGetGLViewController();
    //[contr.view setUserInteractionEnabled:YES]; 
    
    SetInputEnabled(true);
    UnitySendMessage(MUNERIS_OBJECT, "onOfferFailed", "");
}

-(void) didFinishedLoadingTakeover:(NSDictionary*) takeoverInfo
{
    NSLog(@"Finished load takeover for offer");
    UnitySendMessage(MUNERIS_OBJECT, "onOfferLoaded", "");    
}

-(void) didDismissTakeover
{
    if ( takeoverView != nil )
    {
        [takeoverView removeFromSuperview];
        [takeoverView release];
        takeoverView = nil;
    }
    
    //UIViewController* contr = UnityGetGLViewController();
    //[contr.view setUserInteractionEnabled:YES]; 
    
    SetInputEnabled(true);
    UnitySendMessage(MUNERIS_OBJECT, "onOfferClosed", "");   
}

-(void) didClosedOffersView
{
    //UIViewController* contr = UnityGetGLViewController();
    //[contr.view setUserInteractionEnabled:YES];     
    
    SetInputEnabled(true);
    UnitySendMessage(MUNERIS_OBJECT, "onOfferClosed", "");     
}

-(void) didRecievedMessage:(MunerisMessages*) msgs
{
    UnitySendMessage(MUNERIS_OBJECT, "onOfferMessagesReceived", [serializeMessages(msgs) cStringUsingEncoding:NSUTF8StringEncoding]);
}

@end

// INTERFACE BRIDGE //////////////////////////////

extern "C" void _Native_Muneris_Init(const char* json, const char* key, int logLevel)
{
    UIViewController* contr = UnityGetGLViewController();
    adHolder = [[UIView alloc] init];
    [contr.view addSubview:adHolder];
    
    // Deserialize the configuration as a dictionary
    
    NSString* jsonString = [NSString stringWithUTF8String:json];
    NSData* jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    
    NSError* error = nil;
    config = [[[CJSONDeserializer deserializer] deserializeAsDictionary:jsonData error:&error] retain];
    
    apiKey = [[NSString stringWithUTF8String:key] retain];
    
    // Boot muneris with provided JSON
    
    [Muneris setLogLevel:logLevel];
    [Muneris boot:[[[ConfigurationDelegate alloc] init] retain]];
    
    // Add the callbacks
    
    [Muneris addCallback:[[PurchaseDelegate alloc] init]];
    
    // Add notification observers
    
    notificationDelegate = [[[NotificationDelegate alloc] init] retain];
    
    [[NSNotificationCenter defaultCenter] addObserver:notificationDelegate selector:@selector(applicationDidBecomeActive:) name:UIApplicationDidBecomeActiveNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:notificationDelegate selector:@selector(applicationWillResignActive:) name:UIApplicationWillResignActiveNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:notificationDelegate selector:@selector(applicationDidEnterBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:notificationDelegate selector:@selector(applicationWillEnterForeground:) name:UIApplicationWillEnterForegroundNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:notificationDelegate selector:@selector(applicationWillTerminate:) name:UIApplicationWillTerminateNotification object:nil];
}

extern "C" void _Native_Muneris_LoadAdsWithSize(const char* size, const char* zone, int alignment)
{
    MunerisBannerAdsSizes sz;
    
    if ( strcmp(size, "768x90") == 0 )
        sz = MunerisBannerAdsSizes768x90;
    else
        sz = MunerisBannerAdsSizes320x50;
    
    bannerAlignment = alignment;
    
    [Muneris loadAdswithBannerSize:sz withZone:[NSString stringWithUTF8String:zone] withDelegate:[[BannerDelegate alloc] init] withUIViewController:UnityGetGLViewController()];
    
    adsPending = true;
}

extern "C" void _Native_Muneris_LoadAds(const char* zone, int alignment)
{
    const CGRect r = [[UIScreen mainScreen] bounds]; 
    _Native_Muneris_LoadAdsWithSize((r.size.width >= 768) ? "768x90": "320x50", zone, alignment);
}

extern "C" void _Native_Muneris_LoadTakeover(const char* zone)
{
    [Muneris loadTakeover:[NSString stringWithUTF8String:zone] withViewController:UnityGetGLViewController() withDelegate:[[TakeoverDelegate alloc] init]];
}

extern "C" void _Native_Muneris_LogEvent(const char* name, const char* param)
{
    NSMutableDictionary* params = nil;
    
    if ( strlen(param) > 0 )
    {
        NSString* jsonString = [NSString stringWithUTF8String:param];
        NSData* jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
        
        NSError* error = nil;
        params = [[CJSONDeserializer deserializer] deserializeAsDictionary:jsonData error:&error];       
    }
    else
    {
        params = [NSMutableDictionary dictionary];
    }
    
    [Muneris logEvent:[NSString stringWithUTF8String:name] withParameters:params];
}

extern "C" void _Native_Muneris_RequestPurchase(const char* name)
{
    [MunerisIap requestPurchase:[NSString stringWithUTF8String:name]];
}

extern "C" int _Native_Muneris_HasOffers()
{
    return (([Muneris hasOffers] != false) ? 1 : 0);
}

extern "C" void _Native_Muneris_ShowOffers()
{
    UIViewController* contr = UnityGetGLViewController();
   
    SetInputEnabled(false);
    
    [Muneris showOffers:contr withDelegate:[[OfferDelegate alloc] init]];
}

extern "C" void _Native_Muneris_ShowMoreApps()
{
    [Muneris showMoreApps:UnityGetGLViewController()];
}

extern "C" void _Native_Muneris_ShowCustomerSupport()
{
    [Muneris showCustomerSupport:UnityGetGLViewController()];
}

extern "C" void _Native_Muneris_CloseAds()
{
    if ( adView != nil )
    {
        [adHolder setFrame:CGRectMake(0,0,0,0)];
        [adView removeFromSuperview];
        [adView release];
        adView = nil;
    }
    
    adsPending = false;
}

extern "C" int _Native_Muneris_HasMoreApps()
{
    return ([Muneris hasMoreApps] == true) ? 1 : 0;
}

extern "C" void _Native_Muneris_CheckVersion()
{
    [Muneris checkAppVersion];
}

extern "C" void _Native_Muneris_CompleteAction(const char* name)
{
    [Muneris actionComplete:[NSString stringWithUTF8String:name]];
}

extern "C" void _Native_Muneris_CheckMessages(const char* types)
{
    NSArray* parts = [[NSString stringWithUTF8String:types] componentsSeparatedByString:@","];
    NSMutableArray* res = [NSMutableArray array];

    for ( NSString* p in parts )
    {
        if ( [p isEqualToString:@"c"] )
            [res addObject:[NSNumber numberWithInt:MunerisCreditsMessage]];
        else if ( [p isEqualToString:@"t"] )
            [res addObject:[NSNumber numberWithInt:MunerisTextMessage]];        
    }
    
    [Muneris checkMessages:res withDelegate:[[MessageDelegate alloc] init]];
}

extern "C" void _Native_Muneris_ShowAlert(const char* title, const char* text, const char* options)
{
    NSString* jsonString = [NSString stringWithUTF8String:options];
    NSData* jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];    
    
    UIAlertView* msg = [[UIAlertView alloc] initWithTitle:[NSString stringWithUTF8String:title]
                                                  message:[NSString stringWithUTF8String:text]
                                                 delegate:[[AlertDelegate alloc] init]
                                        cancelButtonTitle:nil
                                        otherButtonTitles:nil];
    
    NSError* err = nil;
    alertOptions = [[[CJSONDeserializer deserializer] deserializeAsArray:jsonData error:&err] retain];
    
    int x;
    for ( x = 0; x < [alertOptions count]; x++ )
        [msg addButtonWithTitle:[alertOptions objectAtIndex:x]];
    
    [msg show];
}
