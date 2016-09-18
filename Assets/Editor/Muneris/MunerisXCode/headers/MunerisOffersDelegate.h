//
//  MunerisOffersDelegate.h
//  MunerisSDK
//
//  Created by Casper Lee on 5/6/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisTakeoverDelegate.h"
#import "MunerisInboxDelegate.h"

@protocol MunerisOffersDelegate <NSObject, MunerisTakeoverDelegate, MunerisInboxDelegate>

@required
-(void) didClosedOffersView;

@end
