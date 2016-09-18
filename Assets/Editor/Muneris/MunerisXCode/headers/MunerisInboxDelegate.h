//
//  MunerisInboxDelegate.h
//  MunerisSDK
//
//  Created by Casper Lee on 1/6/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisMessages.h"

@protocol MunerisInboxDelegate <NSObject>

@required
-(void) didRecievedMessage:(MunerisMessages*) msgs;

@end
