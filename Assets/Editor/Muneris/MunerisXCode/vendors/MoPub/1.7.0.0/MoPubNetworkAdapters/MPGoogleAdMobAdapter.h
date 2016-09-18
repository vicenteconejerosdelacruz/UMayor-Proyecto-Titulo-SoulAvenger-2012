//
//  MPGoogleAdMobAdapter.h
//  MoPub
//
//  Created by Andrew He on 5/1/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MPBaseAdapter.h"
#import "GADBannerView.h"

@interface MPGoogleAdMobAdapter : MPBaseAdapter <GADBannerViewDelegate>
{
	GADBannerView *_adBannerView;
}

@end
