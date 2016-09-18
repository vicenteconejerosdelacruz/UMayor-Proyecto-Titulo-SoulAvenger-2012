//
//  MPMillennialAdapter.m
//  MoPub
//
//  Created by Andrew He on 5/1/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import "MPMillennialAdapter.h"
#import "CJSONDeserializer.h"
#import "MPAdView.h"
#import "MPLogging.h"

#define MM_SIZE_320x53	CGSizeMake(320, 53)
#define MM_SIZE_300x250 CGSizeMake(300, 250)
#define MM_SIZE_728x90  CGSizeMake(728, 90)

@interface MPMillennialAdapter ()

@property (nonatomic, retain) MMAdView *mmAdView;
@property (nonatomic, assign) CGSize mmAdSize;
@property (nonatomic, assign) MMAdType mmAdType;
@property (nonatomic, copy) NSString * mmAdApid;

- (void)setAdPropertiesFromNativeParams:(NSDictionary *)params;
- (void)tearDownExistingAdView;

@end

////////////////////////////////////////////////////////////////////////////////////////////////////

@implementation MPMillennialAdapter
@synthesize mmAdView = _mmAdView;
@synthesize mmAdSize = _mmAdSize;
@synthesize mmAdType = _mmAdType;
@synthesize mmAdApid = _mmAdApid;

- (void)dealloc
{
	[self tearDownExistingAdView];
	[super dealloc];
}

- (void)getAdWithParams:(NSDictionary *)params
{
	CJSONDeserializer *deserializer = [CJSONDeserializer deserializerWithNullObject:NULL];
    
    NSData *hdrData = [(NSString *)[params objectForKey:@"X-Nativeparams"] 
					   dataUsingEncoding:NSUTF8StringEncoding];
	NSDictionary *hdrParams = [deserializer deserializeAsDictionary:hdrData error:NULL];
    
	[self setAdPropertiesFromNativeParams:hdrParams];
	[self tearDownExistingAdView];
	
	self.mmAdView = [MMAdView adWithFrame:(CGRect){{0.0, 0.0}, self.mmAdSize} 
									 type:self.mmAdType
									 apid:self.mmAdApid
								 delegate:self
								   loadAd:NO
							   startTimer:NO];
    
    self.mmAdView.rootViewController = [self.delegate viewControllerForPresentingModalView];
	[self.mmAdView refreshAd];
}

- (void)setAdPropertiesFromNativeParams:(NSDictionary *)params { 
    CGFloat width = [(NSString *)[params objectForKey:@"adWidth"] floatValue];
    CGFloat height = [(NSString *)[params objectForKey:@"adHeight"] floatValue];

    if (width == 300.0 && height == 250.0)
    { 
        self.mmAdSize = MM_SIZE_300x250;
        self.mmAdType = MMBannerAdRectangle;
    } 
    else if (width == 728.0 && height == 90.0)
    { 
        self.mmAdSize = MM_SIZE_728x90;
        self.mmAdType = MMBannerAdTop;
    } 
    else
    { 
        self.mmAdSize = MM_SIZE_320x53;
        self.mmAdType = MMBannerAdTop;
    }
    
    self.mmAdApid = [params objectForKey:@"adUnitID"];
}

/* 
 * Safely tears down and releases this adapter's MMAdView, if it exists.
 * Per: http://wiki.millennialmedia.com/index.php/Apple_SDK#adWithFrame
 */
- (void)tearDownExistingAdView
{
	self.mmAdView.refreshTimerEnabled = NO;
	self.mmAdView.delegate = nil;
	self.mmAdView = nil;
}

#pragma mark -
#pragma mark MMAdViewDelegate

- (NSDictionary *)requestData 
{
	NSMutableDictionary *params = [NSMutableDictionary dictionaryWithObjectsAndKeys:
								   @"mopubsdk", @"vendor", nil];
	
	NSArray *locationPair = [[self.delegate adView] locationDescriptionPair];
	if ([locationPair count] == 2) {
		[params setObject:[locationPair objectAtIndex:0] forKey:@"lat"];
		[params setObject:[locationPair objectAtIndex:1] forKey:@"lon"];
	}
	
	return params;
}

- (void)adRequestSucceeded:(MMAdView *)adView
{
	[self.delegate adapter:self didFinishLoadingAd:adView shouldTrackImpression:YES];
}

- (void)adRequestFailed:(MMAdView *)adView
{
	[self.delegate adapter:self didFailToLoadAdWithError:nil];
}

- (void)adWasTapped:(MMAdView *)adView
{
	[self.delegate userActionWillBeginForAdapter:self];
}

- (void)applicationWillTerminateFromAd
{
	[self.delegate userWillLeaveApplicationFromAdapter:self];
}

- (void)adModalWasDismissed
{
	[self.delegate userActionDidFinishForAdapter:self];
}

@end
