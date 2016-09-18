//
//  MPMillennialInterstitialAdapter.h
//  MoPub
//
//  Created by Nafis Jamal on 4/27/11.
//  Copyright 2011 MoPub. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MPBaseInterstitialAdapter.h"
#import "MMAdView.h"


@interface MPMillennialInterstitialAdapter : MPBaseInterstitialAdapter <MMAdDelegate>
{
	MMAdView *_mmInterstitialAdView;
}

@end
