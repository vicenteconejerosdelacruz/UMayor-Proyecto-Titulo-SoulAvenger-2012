//
//  MPMillennialAdapter.h
//  MoPub
//
//  Created by Andrew He on 5/1/11.
//  Copyright 2011 MoPub, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MPBaseAdapter.h"
#import "MMAdView.h"

@interface MPMillennialAdapter : MPBaseAdapter <MMAdDelegate>
{
	MMAdView *_mmAdView;
	NSString *_mmAdApid;
	CGSize _mmAdSize;
	MMAdType _mmAdType;
}

@end
