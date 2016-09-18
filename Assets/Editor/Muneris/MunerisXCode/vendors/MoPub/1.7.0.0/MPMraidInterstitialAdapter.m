//
//  MPMraidInterstitialAdapter.m
//  MoPub
//
//  Created by Andrew He on 12/11/11.
//  Copyright (c) 2012 MoPub, Inc. All rights reserved.
//

#import "MPMraidInterstitialAdapter.h"
#import "MPInterstitialAdController.h"
#import "MPLogging.h"

@implementation MPMraidInterstitialAdapter

- (void)getAdWithParams:(NSDictionary *)params {
    NSData *payload = [params objectForKey:@"payload"];
    NSMutableString *payloadString = [[NSMutableString alloc] initWithData:payload 
                                                                  encoding:NSUTF8StringEncoding];
    
    CGFloat width = [(NSString *)[params objectForKey:@"adWidth"] floatValue];
	CGFloat height = [(NSString *)[params objectForKey:@"adHeight"] floatValue];
    
    // XXX: On iOS 4.3, a divide-by-zero error (CALayerInvalidGeometry) occurs during interstitial
    // presentation if the following two statements are true:
    //     1) the width and/or height of the ad content view is 0,
    //     2) the ad's HTML source has a viewport meta tag that uses the 'initial-scale' attribute.
    // As a workaround, we set any 0-sized dimensions to be 1.
    
    if (width == 0) width = 1;
    if (height == 0) height = 1;
    
    _adView = [[MRAdView alloc] initWithFrame:CGRectMake(0, 0, width, height) 
                              allowsExpansion:NO
                             closeButtonStyle:MRAdViewCloseButtonStyleAdControlled
                                placementType:MRAdViewPlacementTypeInterstitial];
    _adView.delegate = self;
    [_adView loadCreativeWithHTMLString:payloadString baseURL:nil];
    
    [payloadString release];
}

- (void)dealloc {
    _adView.delegate = nil;
    [_adView release];
    [super dealloc];
}

- (void)showInterstitialFromViewController:(UIViewController *)controller {
    if (_loaded) {
        [_interstitialAdController adapter:self requestsPresentationForView:_adView];
    }
}

#pragma mark -
#pragma mark MRAdViewControllerDelegate

- (UIViewController *)viewControllerForPresentingModalView {
    return _interstitialAdController;
}

- (void)adDidLoad:(MRAdView *)adView {
    _loaded = YES;
    [_interstitialAdController adapterDidFinishLoadingAd:self];
}

- (void)adDidFailToLoad:(MRAdView *)adView {
    _loaded = NO;
    [_interstitialAdController adapter:self didFailToLoadAdWithError:nil];
}

- (void)adWillClose:(MRAdView *)adView {
    [_interstitialAdController adapter:self requestsDismissalOfView:adView];
}

- (void)ad:(MRAdView *)adView didRequestCustomCloseEnabled:(BOOL)enabled {
    _interstitialAdController.adWantsNativeCloseButton = !enabled;
}

@end
