//
//  MunerisPurchaseCallback.h
//  MunerisSDK
//
//  Created by Jacky Yuk on 6/21/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisException.h"
#import "MunerisCallback.h"

@protocol MunerisPurchaseCallback <MunerisCallback>

@required

-(void) purchaseDidComplete:(NSString*) sku;

-(void) purchaseDidCancel:(NSString*) sku withUserInfo:(NSDictionary*) dictionary;

-(void) purchaseDidFail:(NSString*) sku withUserInfo:(NSDictionary*) dictionary;


@end
