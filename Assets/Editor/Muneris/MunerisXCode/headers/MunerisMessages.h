//
//  MunerisMessages.h
//  MunerisKit
//
//  Created by Jacky Yuk on 9/19/11.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface MunerisMessages : NSObject {
  
  NSArray* _messages;
  
  NSMutableArray* _unread;
  
}

@property (readonly) NSArray* messages;

@property (readonly) NSArray* readMessage;

@property (readonly) NSArray* unreadMessage;

+(id) messages:(NSArray*) array;

+(NSDictionary*) createCreditMessageWithPlugin:(NSString*) pluginName credits:(int) credits reason:(NSString*)reason;

//-(void) markUnread:(NSDictionary*) dictionary;
//
//-(void) markUnreadWithIndex:(int) index;



@end
