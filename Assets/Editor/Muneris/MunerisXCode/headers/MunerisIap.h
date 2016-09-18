//
//  MunerisIap.h
//  MunerisSDK
//
//  Created by Jacky Yuk on 6/20/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisPurchaseCallback.h"


@interface MunerisIap : NSObject
{
  @private
  NSDictionary* data;
  
  
}

+(void) requestPurchase:(NSString*)sku;


@end
