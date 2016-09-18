//
//  MunerisTakeoverDelegate.h
//  MunerisKit
//
//  Created by Casper Lee on 22/5/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol MunerisTakeoverDelegate <NSObject>

@required
-(BOOL) shouldShowTakeover:(NSDictionary*) takeoverInfo;

-(void) didFailToLoadTakeover:(NSDictionary*) takeoverInfo;

-(void) didFinishedLoadingTakeover:(NSDictionary*) takeoverInfo;

@optional
-(void) didDismissTakeover;

@end
