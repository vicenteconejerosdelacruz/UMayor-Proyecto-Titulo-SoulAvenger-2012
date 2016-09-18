//
//  MPGoogleAdMobInterstitialAdapter.m
//  MoPub
//
//  Created by Nafis Jamal on 4/26/11.
//  Copyright 2011 MoPub. All rights reserved.
//

#import "MPGoogleAdMobInterstitialAdapter.h"
#import "CJSONDeserializer.h"
#import "MPInterstitialAdController.h"
#import "MPLogging.h"

#define kLocationAccuracyMeters 100

@implementation MPGoogleAdMobInterstitialAdapter

- (void)getAdWithParams:(NSDictionary *)params
{
	CJSONDeserializer *deserializer = [CJSONDeserializer deserializerWithNullObject:NULL];
    
    NSData *hdrData = [(NSString *)[params objectForKey:@"X-Nativeparams"] 
					   dataUsingEncoding:NSUTF8StringEncoding];
	NSDictionary *hdrParams = [deserializer deserializeAsDictionary:hdrData error:NULL];
	
	_gAdInterstitial = [[GADInterstitial alloc] init];
	_gAdInterstitial.adUnitID = [hdrParams objectForKey:@"adUnitID"];
	_gAdInterstitial.delegate = self;
	
	GADRequest *request = [GADRequest request];
	
	NSArray *locationPair = [self.interstitialAdController locationDescriptionPair];
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
	
	[_gAdInterstitial loadRequest:request];
}

- (void)interstitialDidReceiveAd:(GADInterstitial *)interstitial
{
	[_interstitialAdController adapterDidFinishLoadingAd:self];
}

- (void)interstitial:(GADInterstitial *)interstitial 
		didFailToReceiveAdWithError:(GADRequestError *)error
{
	[_interstitialAdController adapter:self didFailToLoadAdWithError:error];
}

- (void)showInterstitialFromViewController:(UIViewController *)controller
{
	[_gAdInterstitial presentFromRootViewController:controller];
}

- (void)interstitialWillPresentScreen:(GADInterstitial *)interstitial
{
	[_interstitialAdController interstitialWillAppearForAdapter:self];
	[_interstitialAdController interstitialDidAppearForAdapter:self];
}

- (void)interstitialWillDismissScreen:(GADInterstitial *)ad
{
	[_interstitialAdController interstitialWillDisappearForAdapter:self];
}

- (void)interstitialDidDismissScreen:(GADInterstitial *)ad
{
	[_interstitialAdController interstitialDidDisappearForAdapter:self];
}

- (void)interstitialWillLeaveApplication:(GADInterstitial *)ad
{
    [_interstitialAdController interstitialWasTappedForAdapter:self];
}

- (void)dealloc
{
	_gAdInterstitial.delegate = nil;
	[_gAdInterstitial release];
	[super dealloc];
}

@end
