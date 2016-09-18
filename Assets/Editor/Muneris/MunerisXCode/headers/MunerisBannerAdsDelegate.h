//
//  MunerisBannerAdsDelegate.h
//  MunerisKit
//
//  Created by Casper Lee on 21/5/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "MunerisAdsView.h"

@protocol MunerisBannerAdsDelegate <NSObject>

@required
-(void) didReceiveAds:(MunerisAdsView*)view;

-(void) didFailToReceiveAds:(NSError*)error;

@end
