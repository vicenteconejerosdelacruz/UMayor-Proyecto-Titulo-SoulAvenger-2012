//
//  MPGoogleAdMobAdapter.m
//  MoPub
//
//  Created by Andrew He on 5/1/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import "MPGoogleAdMobAdapter.h"
#import "MPAdManager.h"
#import "MPLogging.h"
#import "CJSONDeserializer.h"

#define kLocationAccuracyMeters	100

@interface MPGoogleAdMobAdapter ()

- (void)setAdPropertiesFromNativeParams:(NSDictionary *)params;

@end

////////////////////////////////////////////////////////////////////////////////////////////////////

@implementation MPGoogleAdMobAdapter

- (id)initWithAdapterDelegate:(id<MPAdapterDelegate>)delegate
{
	if (self = [super initWithAdapterDelegate:delegate])
	{
		_adBannerView = [[GADBannerView alloc] initWithFrame:CGRectZero];
		_adBannerView.delegate = self;
	}
	return self;
}

- (void)dealloc
{
	_adBannerView.delegate = nil;
	[_adBannerView release];
	[super dealloc];
}

- (void)getAdWithParams:(NSDictionary *)params
{
	CJSONDeserializer *deserializer = [CJSONDeserializer deserializerWithNullObject:NULL];
    
    NSData *hdrData = [(NSString *)[params objectForKey:@"X-Nativeparams"] 
					   dataUsingEncoding:NSUTF8StringEncoding];
	NSDictionary *hdrParams = [deserializer deserializeAsDictionary:hdrData error:NULL];
	
	[self setAdPropertiesFromNativeParams:hdrParams];
	_adBannerView.rootViewController = [self.delegate viewControllerForPresentingModalView];
	
	GADRequest *request = [GADRequest request];
	
	NSArray *locationPair = [[self.delegate adView] locationDescriptionPair];
	if ([locationPair count] == 2) {
		[request setLocationWithLatitude:[[locationPair objectAtIndex:0] floatValue] 
							   longitude:[[locationPair objectAtIndex:1] floatValue]
								accuracy:kLocationAccuracyMeters];
	}
	
	// Here, you can specify a list of devices that will receive test ads.
	// See: http://code.google.com/mobile/ads/docs/ios/intermediate.html#testdevices
	request.testDevices = [NSArray arrayWithObjects:
						   GAD_SIMULATOR_ID, 
						   // more UDIDs here,
						   nil];
	
	[_adBannerView loadRequest:request];
}

- (void)setAdPropertiesFromNativeParams:(NSDictionary *)params
{
	CGFloat width = [(NSString *)[params objectForKey:@"adWidth"] floatValue];
	CGFloat height = [(NSString *)[params objectForKey:@"adHeight"] floatValue];
	if (width < GAD_SIZE_320x50.width && height < GAD_SIZE_320x50.height) {
		width = GAD_SIZE_320x50.width;
		height = GAD_SIZE_320x50.height;
	}
	_adBannerView.frame = CGRectMake(0, 0, width, height);
	_adBannerView.adUnitID = [params objectForKey:@"adUnitID"];
}

#pragma mark -
#pragma mark GADBannerViewDelegate methods

- (void)adViewDidReceiveAd:(GADBannerView *)bannerView
{
	[self.delegate adapter:self didFinishLoadingAd:bannerView shouldTrackImpression:YES];
}

- (void)adView:(GADBannerView *)bannerView
		didFailToReceiveAdWithError:(GADRequestError *)error
{
	[self.delegate adapter:self didFailToLoadAdWithError:nil];
}

- (void)adViewWillPresentScreen:(GADBannerView *)bannerView
{
	[self.delegate userActionWillBeginForAdapter:self];
}

- (void)adViewDidDismissScreen:(GADBannerView *)bannerView
{
	[self.delegate userActionDidFinishForAdapter:self];
}

- (void)adViewWillLeaveApplication:(GADBannerView *)bannerView
{
	[self.delegate userWillLeaveApplicationFromAdapter:self];
}

@end
