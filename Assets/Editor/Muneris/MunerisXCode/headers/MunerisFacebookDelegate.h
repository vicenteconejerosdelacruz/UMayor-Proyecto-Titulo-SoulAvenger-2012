//
//  MunerisFacebookDelegate.h
//  MunerisSDK
//
//  Created by Jacky Yuk on 6/19/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Facebook.h"

@protocol MunerisFacebookDelegate <NSObject>

-(void) fbDidReady:(Facebook*) fb;

-(void) fbUserDidCancel:(Facebook*) fb;

-(void) fbDidLogout:(Facebook*) fb;

-(void) fbDidError:(Facebook*)fb;

@end
