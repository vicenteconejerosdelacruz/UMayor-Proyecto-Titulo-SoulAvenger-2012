//
//  MPMillennialInterstitialAdapter.m
//  MoPub
//
//  Created by Nafis Jamal on 4/27/11.
//  Copyright 2011 MoPub. All rights reserved.
//

#import "MPMillennialInterstitialAdapter.h"
#import "MPInterstitialAdController.h"
#import "MMAdView.h"
#import "MPLogging.h"
#import "CJSONDeserializer.h"

@interface MPMillennialInterstitialAdapter ()

+ (MMAdView *)sharedMMAdViewForAPID:(NSString *)apid delegate:(id<MMAdDelegate>)delegate;
- (void)releaseMMAdViewSafely;

@end

////////////////////////////////////////////////////////////////////////////////////////////////////

@implementation MPMillennialInterstitialAdapter

+ (MMAdView *)sharedMMAdViewForAPID:(NSString *)apid
                           delegate:(MPMillennialInterstitialAdapter *)delegate
{
	static NSMutableDictionary *sharedMMAdViews;
	
	if ([apid length] == 0)
	{
		MPLogWarn(@"Failed to create a Millennial interstitial. Have you set a Millennial "
				  @"publisher ID in your MoPub dashboard?");
		return nil;
	}
	
	@synchronized(self)
	{
		if (!sharedMMAdViews) sharedMMAdViews = [[NSMutableDictionary dictionary] retain];
		
		MMAdView *adView = [sharedMMAdViews objectForKey:apid];
		if (!adView)
		{
			adView = [MMAdView interstitialWithType:MMFullScreenAdTransition
											   apid:apid
										   delegate:delegate
											 loadAd:NO];
			[sharedMMAdViews setObject:adView forKey:apid];
		}
        
        [adView setDelegate:delegate];
		return adView;
	}
}

- (void)getAdWithParams:(NSDictionary *)params
{	
    CJSONDeserializer *deserializer = [CJSONDeserializer deserializerWithNullObject:NULL];
    
    NSData *hdrData = [(NSString *)[params objectForKey:@"X-Nativeparams"] 
					   dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *hdrParams = [deserializer deserializeAsDictionary:hdrData error:NULL];
    NSString *apid = [hdrParams objectForKey:@"adUnitID"];
	
	_mmInterstitialAdView = [[[self class] sharedMMAdViewForAPID:apid delegate:self] retain];
	
	if (!_mmInterstitialAdView) {
		[_interstitialAdController adapter:self didFailToLoadAdWithError:nil];
		return;
	}
    
    // If a Millennial interstitial has already been cached, we don't need to fetch another one.
    if ([_mmInterstitialAdView checkForCachedAd]) {
        MPLogInfo(@"Previous Millennial interstitial ad was found in the cache.");
        [_interstitialAdController adapterDidFinishLoadingAd:self];
        return;
    }

    [_mmInterstitialAdView fetchAdToCache];
}

- (void)dealloc
{
	[self releaseMMAdViewSafely];
	[super dealloc];
}

- (void)releaseMMAdViewSafely
{
	if (_mmInterstitialAdView.delegate == self) _mmInterstitialAdView.delegate = nil;
	[_mmInterstitialAdView release]; _mmInterstitialAdView = nil;
}

- (void)showInterstitialFromViewController:(UIViewController *)controller
{
    if ([_mmInterstitialAdView checkForCachedAd])
    {
        _mmInterstitialAdView.rootViewController = controller;
        if (![_mmInterstitialAdView displayCachedAd])
        {
            MPLogInfo(@"Millennial interstitial ad could not be displayed.");
            [_interstitialAdController interstitialDidExpireForAdapter:self];
        }
    }
}

# pragma mark - 
# pragma mark MMAdDelegate

- (NSDictionary *)requestData 
{
	NSMutableDictionary *params = [NSMutableDictionary dictionaryWithObjectsAndKeys:
								   @"mopubsdk", @"vendor", nil];
	
	NSArray *locationPair = [self.interstitialAdController locationDescriptionPair];
	if ([locationPair count] == 2) {
		[params setObject:[locationPair objectAtIndex:0] forKey:@"lat"];
		[params setObject:[locationPair objectAtIndex:1] forKey:@"lon"];
	}
	
	return params;
}

- (void)adRequestFailed:(MMAdView *)adView {
}

- (void)adRequestIsCaching:(MMAdView *)adView {
    MPLogInfo(@"Millennial interstitial ad is currently caching.");
}

- (void)adRequestFinishedCaching:(MMAdView *)adView successful:(BOOL)didSucceed {
    if (didSucceed) {
        MPLogInfo(@"Millennial interstitial ad was cached successfully.");
        [_interstitialAdController adapterDidFinishLoadingAd:self];
    } else {
        MPLogInfo(@"Millennial interstitial ad caching failed.");
        [_interstitialAdController adapter:self didFailToLoadAdWithError:nil];
    }
}

- (void)adModalWillAppear
{
	[_interstitialAdController interstitialWillAppearForAdapter:self];
}

- (void)adModalDidAppear
{
	[_interstitialAdController interstitialDidAppearForAdapter:self];
}

- (void)adModalWasDismissed
{
	[_interstitialAdController interstitialWillDisappearForAdapter:self];
	[_interstitialAdController interstitialDidDisappearForAdapter:self];
    [_interstitialAdController interstitialDidExpireForAdapter:self];
}

@end
