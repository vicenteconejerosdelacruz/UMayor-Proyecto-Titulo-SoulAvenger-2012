//
//  MPCustomEventAdapter.m
//  MoPub
//
//  Created by Andrew He on 2/9/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import "MPCustomEventAdapter.h"
#import "MPAdView.h"
#import "MPInterstitialAdController.h"
#import "MPInterstitialAdController+MPCustomEventAdapterFriend.h"
#import "MPLogging.h"

@implementation MPCustomEventAdapter

- (void)getAdWithParams:(NSDictionary *)params
{
    NSString *selectorString = [params objectForKey:@"X-Customselector"];
    if (!selectorString)
    {
        MPLogError(@"Custom event requested, but no custom selector was provided.",
              selectorString);
        [self.delegate adapter:self didFailToLoadAdWithError:nil];
    }

    SEL selector = NSSelectorFromString(selectorString);
    MPAdView *adView = [self.delegate adView];
    id customEventDelegate = [adView delegate];
    id customEventArgument = adView;
  
    // If the custom event delegate is an interstitial, set the custom event delegate to be the
    // interstitial's delegate, and set the custom event argument to be the interstitial.
    if ([customEventDelegate isKindOfClass:[MPInterstitialAdController class]]) {
        MPInterstitialAdController *controller = (MPInterstitialAdController *)customEventDelegate;
        customEventArgument = controller;
        customEventDelegate = [controller customEventDelegate];
    }
    
    // First, try calling the no-object selector.
    if ([customEventDelegate respondsToSelector:selector])
    {
        [customEventDelegate performSelector:selector];
    }
    // Then, try calling the selector passing in the ad view.
    else 
    {
        NSString *selectorWithObjectString = [NSString stringWithFormat:@"%@:", selectorString];
        SEL selectorWithObject = NSSelectorFromString(selectorWithObjectString);
        
        if ([customEventDelegate respondsToSelector:selectorWithObject])
        {
            [customEventDelegate performSelector:selectorWithObject withObject:customEventArgument];
        }
        else
        {
            MPLogError(@"Ad view delegate does not implement custom event selectors %@ or %@.",
                  selectorString,
                  selectorWithObjectString);
            [self.delegate adapter:self didFailToLoadAdWithError:nil];
        }
    }

}

@end
