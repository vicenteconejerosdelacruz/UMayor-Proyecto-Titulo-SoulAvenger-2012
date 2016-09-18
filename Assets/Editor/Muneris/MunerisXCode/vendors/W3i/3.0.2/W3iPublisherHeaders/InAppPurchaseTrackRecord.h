//
//  InAppPurchaseTrackRecord.h
//  W3iPublisherSdk
//
//  Created by Bozhidar Mihaylov on 4/25/12.
//  Copyright (c) 2012 MentorMate. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface InAppPurchaseTrackRecord : NSObject

//"StoreProductId":"String content",
//"StoreTransactionId":"String content",
//"StoreTransactionTimeUtc":"\/Date(928167600000-0500)\/",
//"CostPerItem":12678967.543233,
//"CurrencyLocale":"String content",
//"Quantity":2147483647,
//"ProductTitle":"String content"

@property (nonatomic, retain) NSString *storeProductID;
@property (nonatomic, retain) NSString *storeTransactionID;
@property (nonatomic, retain) NSDate *storeTransactionTime;
@property (nonatomic, retain) NSDecimalNumber *costPerItem;
@property (nonatomic, retain) NSLocale *currencyLocale;
@property (nonatomic, assign) NSInteger quantity;
@property (nonatomic, retain) NSString *productTitle;

@end
