//
//  InAppPurchaseTrackRequest.h
//  W3iPublisherSdk
//
//  Created by Bozhidar Mihaylov on 4/25/12.
//  Copyright (c) 2012 MentorMate. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "InAppPurchaseTrackRecord.h"

@protocol InAppPurchaseTrackDelegate;
@interface InAppPurchaseTrackRequest : NSObject

@property (nonatomic, retain) InAppPurchaseTrackRecord *trackRecord;
@property (nonatomic, assign) id<InAppPurchaseTrackDelegate> delegate;

@end


@protocol InAppPurchaseTrackDelegate <NSObject>

@optional
- (void)trackInAppPurchaseDidSucceedForRequest:(InAppPurchaseTrackRequest *)inAppPurchaseRequest;
- (void)trackInAppPurchaseForRequest:(InAppPurchaseTrackRequest *)inAppPurchaseRequest didFailWithError:(NSError *)error;

@end

