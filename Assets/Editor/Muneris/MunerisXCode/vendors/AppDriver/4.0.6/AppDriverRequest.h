//
//  AppDriverRequest.h
//  AppDriverLibraryManagement
//
//  Created by qu.xiaoyi on 11/10/03.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

#define kParams @"params"
#define kDomian @"domain"
#define kRetryNum @"retryNum"
#define kMode @"mode"

enum {
    ADVERTISER_REQUEST_MODE = 1,
    PUBLISHER_REQUEST_MODE = 2,
    MONETIZER_REQUEST_MODE = 3
};
typedef NSInteger AppDriverRequestMode;

@interface AppDriverRequest : NSObject <NSCoding>{
    NSMutableDictionary *_params;
    NSString *_domain;
    NSInteger retryNum;
    AppDriverRequestMode mode;
}

@property (nonatomic ,retain) NSMutableDictionary *params;
@property (nonatomic ,retain) NSString *domain;
@property (nonatomic ,assign) NSInteger retryNum;
@property (nonatomic, assign) AppDriverRequestMode mode;

- (NSURLRequest *)request;
- (id)initWithMode:(AppDriverRequestMode)currentMode AndParams:(NSMutableDictionary *)params;
- (void)open;

@end
