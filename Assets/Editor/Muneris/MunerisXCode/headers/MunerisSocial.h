//
//  MunerisSocialManager.h
//  MunerisSDK
//
//  Created by Casper Lee on 18/6/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisFacebookDelegate.h"

@interface MunerisSocial : NSObject

+(MunerisSocial*) getManager;

+(void) fbAccess:(id<MunerisFacebookDelegate>)delegate;

+(void) fbLogout:(id<MunerisFacebookDelegate>)delegate;

+(BOOL) isFbLoggedIn;

@end
