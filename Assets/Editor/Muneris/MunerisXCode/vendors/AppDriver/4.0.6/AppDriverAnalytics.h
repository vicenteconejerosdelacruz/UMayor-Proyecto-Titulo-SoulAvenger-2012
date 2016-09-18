//
//  AppDriverAnalytics.h
//  AppDriverLibraryManagement
//
//  Created by qu.xiaoyi on 11/10/05.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import "AppDriverRequest.h"

@interface AppDriverAnalytics : NSObject

+ (AppDriverRequest *)getPaymentRequestWithProduction:(SKProduct *)product AndTransaction:(SKPaymentTransaction  *)transaction;
+ (AppDriverRequest *)getPaymentRequestWithTransaction:(SKPaymentTransaction  *)transaction;

@end
