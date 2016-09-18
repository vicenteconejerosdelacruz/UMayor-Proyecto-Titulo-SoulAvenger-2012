//
//  MPIAdInterstitialAdapter.h
//  MoPub
//
//  Created by Haydn Dufrene on 10/28/11.
//  Copyright 2011 MoPub. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MPBaseInterstitialAdapter.h"
#import <iAd/iAd.h>

@interface MPIAdInterstitialAdapter : MPBaseInterstitialAdapter <ADInterstitialAdDelegate>
{
    ADInterstitialAd *_iAdInterstitial;
    BOOL _isOnscreen;
}

@end
