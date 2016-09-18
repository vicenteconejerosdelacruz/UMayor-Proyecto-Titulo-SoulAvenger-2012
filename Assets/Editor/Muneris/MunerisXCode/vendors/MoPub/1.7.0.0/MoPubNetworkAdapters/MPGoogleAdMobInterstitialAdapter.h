//
//  MPGoogleAdMobInterstitialAdapter.h
//  MoPub
//
//  Created by Nafis Jamal on 4/26/11.
//  Copyright 2011 MoPub. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "GADInterstitial.h"
#import "MPBaseInterstitialAdapter.h"

@interface MPGoogleAdMobInterstitialAdapter : MPBaseInterstitialAdapter <GADInterstitialDelegate> 
{
	GADInterstitial *_gAdInterstitial;
}

@end
