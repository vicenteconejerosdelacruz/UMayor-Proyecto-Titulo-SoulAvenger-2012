//
//  MPInterstitialAdController+MPCustomEventAdapterFriend.h
//  MoPub
//
//  Created by Andrew He on 6/25/12.
//  Copyright (c) 2012 MoPub, Inc. All rights reserved.
//

#import "MPInterstitialAdController.h"

@interface MPInterstitialAdController (MPCustomEventAdapterFriend)

- (id<MPInterstitialAdControllerDelegate>)customEventDelegate;

@end
