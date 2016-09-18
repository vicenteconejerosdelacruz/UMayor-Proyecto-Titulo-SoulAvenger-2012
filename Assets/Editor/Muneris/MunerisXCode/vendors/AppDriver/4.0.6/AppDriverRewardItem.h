//
//  AppDriverRewardItem.h
//  AppDriverLibraryManagement
//
//  Created by qu.xiaoyi on 11/10/06.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface AppDriverRewardItem : NSObject {
    NSString *identifier;
    NSInteger price;
    NSString *name;
    NSString *imgURL;
}

@property (nonatomic, retain) NSString *identifier;
@property (nonatomic, assign) NSInteger price;
@property (nonatomic, retain) NSString *name;
@property (nonatomic, retain) NSString *imgURL;

- (id)initWithIdentifier:(NSString *)itemIdentifier Price:(NSInteger)itemPrice Name:(NSString *)itemName ImageURL:(NSString *)itemImageURL;

@end
